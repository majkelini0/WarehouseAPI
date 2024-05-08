using WarehouseButBetter.Models;

namespace WarehouseButBetter.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExists(ProductWarehouse productWarehouse);
    Task<bool> DoesWarehouseExists(ProductWarehouse productWarehouse);
    Task<bool> IsSuchOrder(ProductWarehouse productWarehouse);
}