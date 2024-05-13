using System.Data;
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

    public async Task<bool> DoesProductExists(ProductWarehouseRequestModel productWarehouseRequestModel)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        var query = "SELECT 1 FROM Product where idproduct = @IdProduct";
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", productWarehouseRequestModel.IdProduct);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesWarehouseExists(ProductWarehouseRequestModel productWarehouseRequestModel)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        var query = "SELECT 1 FROM Warehouse where idwarehouse = @IdWarehouse";
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdWarehouse", productWarehouseRequestModel.IdWarehouse);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<int?> IsSuchOrder(ProductWarehouseRequestModel productWarehouseRequestModel)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        var query = "SELECT IdOrder FROM [Order] where idproduct = @IdProduct and amount = @Amount";
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", productWarehouseRequestModel.IdProduct);
        command.Parameters.AddWithValue("@Amount", productWarehouseRequestModel.Amount);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();

        if (res == null)
        {
            return null;
        }

        return (int)res;
    }

    public async Task<bool> WasOrderFulfilled(int idOrder)
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

    public async Task<bool> UpdateOrderFulfilledAt(int idOrder)
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

    public async Task<int?> InsertIntoProductWarehouse(ProductWarehouseRequestModel productWarehouseRequestModel, int idOrder)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        command.Parameters.AddWithValue("@IdProduct", productWarehouseRequestModel.IdProduct);
        
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
            var totalPrice = (decimal)price * productWarehouseRequestModel.Amount;
            
            command.CommandText = "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                                  "OUTPUT INSERTED.IdProductWarehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, GETDATE())";
            command.Parameters.AddWithValue("@IdWarehouse", productWarehouseRequestModel.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", productWarehouseRequestModel.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@Amount", productWarehouseRequestModel.Amount);
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

    public async Task<int?> InsertIntoProductWarehouseProcedure(ProductWarehouseRequestModel productWarehouseRequestModel)
    {
        // Zalozenie: procedura AddProductToWarehouse znajduje sie w bazie danych !
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand("AddProductToWarehouse", connection);
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@IdProduct", productWarehouseRequestModel.IdProduct);
        command.Parameters.AddWithValue("@IdWarehouse", productWarehouseRequestModel.IdWarehouse);
        command.Parameters.AddWithValue("@Amount", productWarehouseRequestModel.Amount);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        
        await connection.OpenAsync();
        var idProductWarehouse = await command.ExecuteScalarAsync();

        if (idProductWarehouse == null)
        {
            return null;
        }
        return (int) idProductWarehouse;
    }
}