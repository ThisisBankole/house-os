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
[Authorize]

public class AnalyticsController : ControllerBase
{
    private readonly IGroceryService _groceryService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IGroceryService groceryService, ILogger<AnalyticsController> logger)
    {
        _groceryService = groceryService;
        _logger = logger;
    }

    // GET: api/Analytics
    [HttpGet]
    public async Task<ActionResult<AnalyticsData>> GetAnalytics()
    {
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("User ID not found or invalid.");
        }

        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var tomorrowStart = todayStart.AddDays(1);
        var sevenDaysAgo = now.AddDays(-7); 
        var thirtyDaysAgo = now.AddDays(-30);
        var threeHundredSixtyFiveDaysAgo = now.AddDays(-365);
        var allTime = DateTime.MinValue;

        var today = await _groceryService.GetAnalyticsDataAsync(userId, todayStart, tomorrowStart);
        var thisWeek = await _groceryService.GetAnalyticsDataAsync(userId, sevenDaysAgo, tomorrowStart);
        var thisMonth = await _groceryService.GetAnalyticsDataAsync(userId, thirtyDaysAgo, tomorrowStart);
        var thisYear = await _groceryService.GetAnalyticsDataAsync(userId, threeHundredSixtyFiveDaysAgo, tomorrowStart);
        var allTimeData = await _groceryService.GetAnalyticsDataAsync(userId, allTime, tomorrowStart);



        var analyticsData = new AnalyticsData
        {
            Today = today,
            ThisWeek = thisWeek,
            ThisMonth = thisMonth,
            ThisYear = thisYear,
            AllTime = allTimeData
        };

        return Ok(analyticsData);
    }

    // GET: api/Analytics/period
    [HttpGet("custom")]
    public async Task<ActionResult<AnalyticsPeriod>> GetCustomAnalytics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("User ID not found or invalid.");
        }

        var utcStartDate = startDate.ToUniversalTime();
        var utcEndDate = endDate.ToUniversalTime();

        var data = await _groceryService.GetAnalyticsDataAsync(userId, utcStartDate, utcEndDate);
        return Ok(data);
    }

    // GET: api/Analytics/popular
    [HttpGet("popular")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<PopularItems>>> GetPopularItems()
    {
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("User ID not found or invalid.");
        }

        var popularItems = await _groceryService.GetPopularItemsAsync(userId, 10);
        return Ok(popularItems);
    }

    // GET: api/Analytics/pricetrends
    [HttpGet("pricetrends")]
    [Authorize]
    public async Task<ActionResult<Dictionary<string, List<PriceTrend>>>> GetPriceTrends([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("User ID not found or invalid.");
        }

        // var utcStartDate = startDate.ToUniversalTime();
        // var utcEndDate = endDate.ToUniversalTime();

        var priceTrends = await _groceryService.GetPriceTrendsAsync(userId);
        Console.WriteLine($"Price Trends: {System.Text.Json.JsonSerializer.Serialize(priceTrends)}");
        return Ok(priceTrends);
    }
   

}
