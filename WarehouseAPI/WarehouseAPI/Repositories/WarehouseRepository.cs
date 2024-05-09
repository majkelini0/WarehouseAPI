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

    public async Task<int?> InsertIntoProductWarehouse(ProductWarehouse productWarehouse, int idOrder)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
        
        await connection.OpenAsync();
        DbTransaction transaction = connection.BeginTransaction();
        command.Transaction = (SqlTransaction)transaction;

        try
        {
            var price = await command.ExecuteScalarAsync();
            command.Parameters.Clear();
            
            if (price == null)
            {
                return null;
            }
            var totalPrice = (decimal)price * productWarehouse.Amount;
            
            command.CommandText = "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                                  "OUTPUT INSERTED.IdProductWarehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, GETDATE())";
            command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            command.Parameters.AddWithValue("@Price", totalPrice);

            var idProductWarehouse = await command.ExecuteScalarAsync();
            if (idProductWarehouse == null)
            {
                return null;
            }
            
            await transaction.CommitAsync();
            return (int) idProductWarehouse;
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            await transaction.RollbackAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await transaction.RollbackAsync();
        }
        return null;
    }
}