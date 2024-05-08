namespace WarehouseButBetter.Models;

public class ProductWarehouseOld
{
    public int IdProductWarehouse { get; set; }
    public int IdWarehouse { get; set; }
    public int IdProduct { get; set; }
    public int IdOrder { get; set; }
    public int Amount { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record Product
{
    public int IdProduct { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }
}

public record Warehouse
{
    public int IdWarehouse { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

public record Order
{
    public int IdOrder { get; set; }
    public int IdProduct { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime FulfilledAt { get; set; }
}