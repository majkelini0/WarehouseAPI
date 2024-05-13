using WarehouseButBetter.Models;

namespace WarehouseButBetter.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExists(ProductWarehouseRequestModel productWarehouseRequestModel);
    Task<bool> DoesWarehouseExists(ProductWarehouseRequestModel productWarehouseRequestModel);
    Task<int?> IsSuchOrder(ProductWarehouseRequestModel productWarehouseRequestModel);
    Task<bool> WasOrderFulfilled(int idOrder);
    Task<bool> UpdateOrderFulfilledAt(int idOrder);
    Task<int?> InsertIntoProductWarehouse(ProductWarehouseRequestModel productWarehouseRequestModel, int idOrder);
    Task<int?> InsertIntoProductWarehouseProcedure(ProductWarehouseRequestModel productWarehouseRequestModel);
}