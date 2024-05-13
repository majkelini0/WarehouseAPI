using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.Models;

public class Warehouse
{
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
}