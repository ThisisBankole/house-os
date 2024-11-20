using HouseOs.Data;
using HouseOs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseOs.Services;

public class UserService: IUserService
{
    private readonly AppSettings _appSettings;
    private readonly ApplicationContext _context;

    public UserService(IOptions<AppSettings> appSettings, ApplicationContext context)
    {
        _appSettings = appSettings.Value;
        _context = context;
    }

    // Authenticate user
    public async Task<AuthenticateResponse?> AuthenticateAsync(AuthenticateRequest model)
    {
        var user = await _context.Users
        .FirstOrDefaultAsync(x => x.Email == model.Email);

        // return null if user not found or password is incorrect
        if(user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            return null;
        }

        // authentication successful so generate jwt token
        var token = generateJwtToken(user);

        return new AuthenticateResponse(user, token);

    }

    // Get all users
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    // Get user by id

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    // Add or update user
    public async Task<User?> AddAndUpdateUserAsync(User userObj)
    {
        bool isSuccess = false;
        if(userObj.Id > 0)
        {
            var obj = await _context.Users.FirstOrDefaultAsync(x => x.Id == userObj.Id);
            if (obj != null)
            {
                obj.FirstName = userObj.FirstName;
                obj.LastName = userObj.LastName;
               // obj.Email = userObj.Email;

                if (!string.IsNullOrEmpty(userObj.Password))
                {
                    obj.Password = BCrypt.Net.BCrypt.HashPassword(userObj.Password);
                }

                _context.Users.Update(obj);
                isSuccess = await _context.SaveChangesAsync() > 0;
            }

        }
        else
        {
            userObj.Password = BCrypt.Net.BCrypt.HashPassword(userObj.Password);
            await _context.Users.AddAsync(userObj);
            isSuccess = await _context.SaveChangesAsync() > 0;
        }

        return isSuccess ? userObj : null;
    }

    

    // helper method to generate jwt token
    private string generateJwtToken(User user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
        }
    }


