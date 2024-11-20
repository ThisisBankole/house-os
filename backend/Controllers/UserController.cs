using HouseOs.Models;
using HouseOs.Services;
using Microsoft.AspNetCore.Mvc;
using HouseOs.Helpers;
using Microsoft.Extensions.Logging;

namespace HouseOs.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserController: ControllerBase
{

    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }


    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticateResponse>> Authenticate( [FromBody] AuthenticateRequest model)
    {
        var response = await _userService.AuthenticateAsync(model);

        if (response == null)
        {
            return BadRequest(new { message = "Email or password is incorrect" });
        }

        return Ok(response);
    }

    [HttpGet("/getall")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("add")]    
    public async Task<ActionResult<User>> AddUser( [FromBody] User userObj)
    {
       
        userObj.Id = 0;
        var response = await _userService.AddAndUpdateUserAsync(userObj);
        if (response != null)
        {
             _logger.LogInformation("User added successfully: {@Response}", response);
            response.Password = null!;
            return Ok(response);
        }
        _logger.LogWarning("Failed to add user: {@UserObj}", userObj);
        return BadRequest(new { message = "User not added." });
        
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User userObj)
    {
        userObj.Id = id;
        var response = await _userService.AddAndUpdateUserAsync(userObj);
       return Ok(response);
    }


}
