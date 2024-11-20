using System;
using System.ComponentModel.DataAnnotations;

namespace HouseOs.Models;

public class GroceryDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }


    // UserId should not be set by the client, it should be set by the server based on the authenticated user
    public int UserId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }


    [Required(ErrorMessage = "TotalCost is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "TotalCost must be greater than 0")]
    public decimal TotalCost { get; set; }

    // UnitCost can be calculated, so we don't need it in the input
    public decimal UnitCost { get; private set; }

    public DateTime CreatedAt { get; set; }

    public GroceryDTO()
    {
        CreatedAt = DateTime.UtcNow;

    }

    // Method to calculate UnitCost
    public void CalculateUnitCost()
    {
        UnitCost = Quantity > 0 ? TotalCost / Quantity : 0;
    }
}