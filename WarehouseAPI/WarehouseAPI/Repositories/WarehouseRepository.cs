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

    public async Task<bool> IsSuchOrder(ProductWarehouse productWarehouse)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        var query = "SELECT 1 FROM [Order] where idproduct = @IdProduct and amount = @Amount";
        command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
        command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
        
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }
}