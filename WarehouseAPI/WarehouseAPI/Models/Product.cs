using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.Models;

public class Product
{
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public Decimal Price { get; set; }
}