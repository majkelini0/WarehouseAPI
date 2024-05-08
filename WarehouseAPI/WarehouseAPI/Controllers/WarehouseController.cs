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
        
        if(await _warehouseRepository.IsSuchOrder(productWarehouse) == false)
        {
            return NotFound($"There is no such order");
        }
        
        
        
        
        
        // int result;
        // try
        // {
        //     result = await _warehouseRepository.UpdateProductWarehouse(productWarehouse);
        // }
        // catch (Exception e)
        // {
        //     return StatusCode(StatusCodes.Status404NotFound);
        // }
        // return Ok(result);
        return Ok();
    }
}