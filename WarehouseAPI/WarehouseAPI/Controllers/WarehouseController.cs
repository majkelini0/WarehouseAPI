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
    
    [HttpPost]
    public async Task<IActionResult> UpdateProductWarehouse(ProductWarehouse productWarehouse)
    {
        if(await _warehouseRepository.DoesProductExists(productWarehouse) == false)
        {
            return NotFound($"Product with given id does not exist");
        }

        if (await _warehouseRepository.DoesWarehouseExists(productWarehouse) == false)
        {
            return NotFound($"Warehouse with given id does not exist");
        }
        
        int? res = await _warehouseRepository.IsSuchOrder(productWarehouse);
        if(res == null)
        {
            return NotFound($"There is no such order");
        }

        if (res.HasValue && await _warehouseRepository.WasOrderFulfilled(productWarehouse, res.Value) == true)
        {
            return Conflict("The order has already been fulfilled");
        }
        
        if(await _warehouseRepository.UpdateOrderFulfilledAt(productWarehouse, res.Value) == false)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to update Order FulfilledAt datetime");
        }
        
        return Ok();
    }
}