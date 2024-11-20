using System;
using HouseOs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using HouseOs.Services;
using System.Security.Claims;

namespace HouseOs.Helpers;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;
    private readonly ILogger<JwtMiddleware> _logger;

    private readonly IEnumerable<string> _exemptPaths;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, ILogger<JwtMiddleware> logger, IEnumerable<string> exemptPaths)
    {
        _next = next;
        _appSettings = appSettings.Value;
        _logger = logger;
        _exemptPaths = exemptPaths;
    }
    public async Task Invoke(HttpContext context, IUserService userService)
    {

        var path = context.Request.Path;
        if (_exemptPaths.Any(p => path.StartsWithSegments(p)))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        Console.WriteLine($"JwtMiddleware: Received token: {token?.Substring(0, Math.Min(token?.Length ?? 0, 10))}...");

        if (token != null)
        {
            await attachUserToContext(context, userService, token);
        }
        else
        {
            _logger.LogWarning("No JWT token found in request");
        }

        await _next(context);
        
    }

    private async Task attachUserToContext(HttpContext context, IUserService userService, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var validationParameters =  new TokenValidationParameters
            {

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };


           _logger.LogInformation("Attempting to validate JWT token");
           var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            _logger.LogInformation("JWT token validated successfully");
          
            var userIdClaim = claimsPrincipal.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogWarning("JWT token does not contain a valid user ID");
                return;
            }

            _logger.LogInformation($"User ID from token: {userId}");

            var user = await userService.GetByIdAsync(userId);
            if (user != null)
            {
                // Create a new identity with the authentication type explicitly set
                var identity = new ClaimsIdentity(claimsPrincipal.Claims, "Bearer");

                // Create a new ClaimsPrincipal with the identity
                context.User = new ClaimsPrincipal(identity);

                // Also set the User in HttpContext.Items for consistency
                context.Items["User"] = user;

                _logger.LogInformation($"User authenticated and attached to context: {userId}");
            }
            else
            {
                _logger.LogWarning($"User with ID {userId} not found in the database");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"JWT validation failed: {ex.Message}");
        }


    }

}
