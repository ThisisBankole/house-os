using System.ComponentModel.DataAnnotations;

namespace HouseOs.Models;

public class CreateGroceryDTO
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "TotalCost is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "TotalCost must be greater than 0")]
    public decimal TotalCost { get; set; }

}
