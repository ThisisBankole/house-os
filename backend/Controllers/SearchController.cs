using System;
using Microsoft.AspNetCore.Mvc;
using HouseOs.Services;
using HouseOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HouseOs.Helpers;

namespace HouseOs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly IGroceryService _groceryService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(IGroceryService groceryService, ILogger<SearchController> logger)
    {
        _groceryService = groceryService;
        _logger = logger;
    }

    // GET: api/Search/search?name={name}
    [HttpGet("search")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Grocery>>> SearchGroceries([FromQuery] string name)
    {
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("User ID not found or invalid.");
        }
        var groceries = await _groceryService.SearchGroceriesAsync(name, userId);
        if (groceries == null)
        {
            return NotFound("No groceries found.");
        }
        return Ok(groceries ?? new List<Grocery>());
    }
}
