using WarehouseButBetter.Models;

namespace WarehouseButBetter.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExists(ProductWarehouse productWarehouse);
    Task<bool> DoesWarehouseExists(ProductWarehouse productWarehouse);
    Task<int?> IsSuchOrder(ProductWarehouse productWarehouse);
    Task<bool> WasOrderFulfilled(ProductWarehouse productWarehouse, int idOrder);
    Task<bool> UpdateOrderFulfilledAt(ProductWarehouse productWarehouse, int idOrder);
    Task<int?> InsertIntoProductWarehouse(ProductWarehouse productWarehouse, int idOrder);
}