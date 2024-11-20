using System.ComponentModel;

namespace HouseOs.Models;

public class AuthenticateRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
