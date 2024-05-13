using System.Net;
using Microsoft.AspNetCore.Mvc;
using WarehouseButBetter.Repositories;
using WarehouseButBetter.Models;

namespace WarehouseButBetter.Controllers;

[ApiController]
[Route("MKWarehouseAPI/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;
    
    public WarehouseController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    [HttpPost("UpdateProductWarehouse")]
    public async Task<IActionResult> UpdateProductWarehouse(ProductWarehouseRequestModel productWarehouseRequestModel)
    {
        if(await _warehouseRepository.DoesProductExists(productWarehouseRequestModel) == false)
        {
            return NotFound($"Product with given id does not exist");
        }

        if (await _warehouseRepository.DoesWarehouseExists(productWarehouseRequestModel) == false)
        {
            return NotFound($"Warehouse with given id does not exist");
        }
        
        int? res = await _warehouseRepository.IsSuchOrder(productWarehouseRequestModel);
        if(res == null)
        {
            return NotFound($"There is no such order");
        }

        if (await _warehouseRepository.WasOrderFulfilled(res.Value) == true)
        {
            return Conflict("The order has already been fulfilled");
        }
        
        if(await _warehouseRepository.UpdateOrderFulfilledAt(res.Value) == false)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to update Order FulfilledAt datetime");
        }

        
        int? idProductWarehouse = await _warehouseRepository.InsertIntoProductWarehouse(productWarehouseRequestModel, res.Value);
        if (idProductWarehouse == null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to insert into ProductWarehouse table");
        }
        
        return Ok(idProductWarehouse);
    }

    [HttpPost("UpdateProductWarehouseProcedure")]
    public async Task<IActionResult> UpdateProductWarehouseProcedure(ProductWarehouseRequestModel productWarehouseRequestModel)
    {
        int? idProductWarehouse = await _warehouseRepository.InsertIntoProductWarehouseProcedure(productWarehouseRequestModel);
        Console.WriteLine(idProductWarehouse);
        if (idProductWarehouse == null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to insert into ProductWarehouse table");
        }

        return Ok(idProductWarehouse);
    }
}