using Microsoft.AspNetCore.Mvc;
using HouseOs.Services;
using HouseOs.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using HouseOs.Helpers;

namespace HouseOs.Controllers;



[Route("api/[controller]")]
[ApiController]

public class GroceryController: ControllerBase
{
    private readonly IGroceryService _groceryService;
    private readonly ILogger<GroceryController> _logger;


    public GroceryController(IGroceryService groceryService, ILogger<GroceryController> logger)
    {
        _groceryService = groceryService;
        _logger = logger;
       
    }

    // GET: api/Grocery
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Grocery>>> GetGroceries()
    {
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("User ID not found or invalid.");
        }

        _logger.LogInformation("User ID: {UserId}", userId);

        var groceries = await _groceryService.GetGroceriesAsync(userId);

        if (groceries == null)
        {
            return NotFound("No groceries found.");
        }

        return Ok(groceries ?? new List<Grocery>());
    }

    // GET: api/Grocery/5
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Grocery>> GetGrocery(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("User ID not found.");
            }
           
            var grocery = await _groceryService.GetGroceryByIdAsync(id, userId);
            return Ok(grocery);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // PUT: api/Grocery
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateGroceryAsync(int id, GroceryDTO obj)
    {
        if (!await _groceryService.GroceryExistsAsync(id))
        {
            return NotFound("Grocery not found.");
        }

        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("User ID not found.");
        }
        var updatedGrocery = await _groceryService.UpdateGroceryAsync(id, obj, userId);

        if (updatedGrocery == null)
        {
            return NotFound("Grocery not found.");
        }
        return NoContent();
    }

    // POST: api/Grocery
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<GroceryDTO>> CreateGroceryAsync(CreateGroceryDTO createDto)
    {

        _logger.LogInformation("CreateGroceryAsync called");
        if (createDto == null)
            {
                _logger.LogWarning("Received null GroceryDTO");
                return BadRequest("The grocery object is required.");
            }

        if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid ModelState: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

        _logger.LogInformation("Received grocery: {Grocery}", System.Text.Json.JsonSerializer.Serialize(createDto));

        _logger.LogInformation("User claims: {Claims}", string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}")));

        var userIdClaim = User.FindFirst("id");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            _logger.LogWarning("User ID claim not found or invalid");
            return BadRequest("User ID not found or invalid.");
        }

        _logger.LogInformation("User ID: {UserId}", userId);

        var groceryDto = new GroceryDTO
        {
            Name = createDto.Name,
            Quantity = createDto.Quantity,
            TotalCost = createDto.TotalCost,
            UserId = userId
        };
        groceryDto.CalculateUnitCost();


        var newGroceries = await _groceryService.CreateGroceryAsync(groceryDto, userId);

        if (newGroceries == null)
        {
            return NotFound("No grocery found.");
        }
        return CreatedAtAction(nameof(GetGrocery), new { id = newGroceries.Id }, newGroceries);
    }

    // DELETE: api/Grocery/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroceryAsync(int id)
    {
        if (!await _groceryService.GroceryExistsAsync(id))
        {
            return NotFound("Grocery not found.");
        }
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("User ID not found.");
        }

        var deletedGrocery = await _groceryService.DeleteGroceryAsync(id, userId);

        if (!deletedGrocery)
        {
            return NotFound("Grocery not found.");
        }
        return NoContent();
    }

    // POST: api/Grocery/ScanReceipt
    [Authorize]
    [HttpPost("Scan")]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> ScanReciept(IFormFile file)
    {
        if (file == null)
        {
            return BadRequest("No file found.");
        }

       var userIdClaim = User.FindFirstValue("id");

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        
        {
            return BadRequest("User not found.");
        }
        using var stream = file.OpenReadStream();
        var groceries = await _groceryService.ScanReceiptAsync(stream, userId);
        return Ok(groceries);
    }

    // upload json of groceries
    [Authorize]
    [HttpPost("Upload")]

    public async Task<IActionResult> UploadGrocery(IFormFile file)
    {
        if (file == null)
        {
            return BadRequest("No file found.");
        }

        if (file.ContentType != "application/json")
        {
            return BadRequest("Invalid file type.");
        }

        var userIdClaim = User.FindFirstValue("id");

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return BadRequest("User not found.");
        }

        try
        {
            var groceries = await _groceryService.UploadGroceryAsync(file, userId);
            return Ok(groceries);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

   

    

}
