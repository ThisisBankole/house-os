using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HouseOs.Models;
using HouseOs.Data;
using HouseOs.Services;
using HouseOs.Helpers;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using HouseOs.Controllers;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment.EnvironmentName;

string connectionString;
if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException($"Connection string not found for environment: {env}");
}
else
{
    // In Azure, use the connection string from app settings
    connectionString = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection") ?? throw new InvalidOperationException($"Connection string not found for environment: {env}");
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException($"Connection string not found for environment: {env}");
}



// Add services to the container.




builder.Services.AddControllers()
    .AddApplicationPart(typeof(UserController).Assembly); // This is crucial for API controllers
//builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins(
            "http://localhost:3000", 
            "https://brave-beach-08974a20f.5.azurestaticapps.net", 
            "https://green-plant-0b4dd561e.5.azurestaticapps.net"
            "https://*.azurestaticapps.net"  
            ) // Adjust if your React app runs on a different port
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Configure the database context



builder.Services.AddSpaStaticFiles(c =>
{
    c.RootPath = "client/build";
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HouseOs", Version = "v1" });
    
    // JWT configuration (as you had it before)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});



builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(60);
    });
});




builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IGroceryService, GroceryService>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HouseOs v1"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowReactApp"); // Use CORS before routing
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

var exemptPaths = new List<string> { "/", "/swagger", "/api-docs", "/api-docs/swagger.json", "/api-docs/swagger.yaml", "/api/add", "/api/authenticate" };
app.UseMiddleware<JwtMiddleware>(exemptPaths);


app.MapControllers(); 

app.Run();


