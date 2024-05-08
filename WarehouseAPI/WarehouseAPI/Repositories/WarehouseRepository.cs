using System.Data.Common;
using Microsoft.Data.SqlClient;
using WarehouseButBetter.Models;

namespace WarehouseButBetter.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesProductExists(ProductWarehouse productWarehouse)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        var query = "SELECT 1 FROM Product where idproduct = @IdProduct";
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesWarehouseExists(ProductWarehouse productWarehouse)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        var query = "SELECT 1 FROM Warehouse where idwarehouse = @IdWarehouse";
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<int?> IsSuchOrder(ProductWarehouse productWarehouse)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        var query = "SELECT IdOrder FROM [Order] where idproduct = @IdProduct and amount = @Amount";
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
        command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        if (res == null)
        {
            return null;
        }

        return (int)res;
    }

    public async Task<bool> WasOrderFulfilled(ProductWarehouse productWarehouse, int idOrder)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        var query = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        if (res != null)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateOrderFulfilledAt(ProductWarehouse productWarehouse, int idOrder)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        var query = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder";
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();
        int res = await command.ExecuteNonQueryAsync();
        
        if (res == 0)
        {
            return false;
        }
        return true;
    }
}