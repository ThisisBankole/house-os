using HouseOs.Models;


namespace HouseOs.Services;

public interface IUserService
{
    Task <AuthenticateResponse?> AuthenticateAsync(AuthenticateRequest model);

    Task <IEnumerable<User>> GetAllAsync();

    Task <User?> GetByIdAsync(int id);

    Task<User?> AddAndUpdateUserAsync(User userObj);

}
