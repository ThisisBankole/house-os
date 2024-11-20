using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseOs.Models;

public class Grocery
{
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(255)")]
    public required string Name { get; set; }

    public int UserId { get; set; } 

    private decimal _quantity;
    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity 
    {
        get => _quantity;
        set 
        { 
            _quantity = value; 
            CalculateUnitCost();
        }
    }

    [Required]
    [Range(0.01, double.MaxValue)]
    [DataType(DataType.Currency)]
    [Display(Name = "Total Cost")]
    [Column(TypeName = "decimal(18,2)")]
    private decimal _totalCost;
    public decimal TotalCost 
    { 
        get => _totalCost;
        set 
        { 
            _totalCost = value; 
            CalculateUnitCost();
        }
    }

    [Required]
    [Range(0.01, double.MaxValue)]
    [DataType(DataType.Currency)]
    [Display(Name = "Unit Cost")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCost { get; private set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Grocery()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private void CalculateUnitCost()
    {
        UnitCost = Quantity == 0 ? 0 : TotalCost / Quantity;
        UnitCost = Math.Round(UnitCost, 2);
    }
}