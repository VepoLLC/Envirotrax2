
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/water-suppliers")]
public class WaterSupplierController : AdminBaseController
{
    private readonly IWaterSupplierService _waterSupplierService;

    public WaterSupplierController(IWaterSupplierService waterSupplierService)
    {
        _waterSupplierService = waterSupplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var suppliers = await _waterSupplierService.GetAllAsync(pageInfo, query, cancellationToken);
        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var details = await _waterSupplierService.GetDetailsAsync(id, cancellationToken);

        if (details == null)
        {
            return NotFound();
        }

        return Ok(details);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, WaterSupplierDetailsDto details)
    {
        var updated = await _waterSupplierService.UpdateDetailsAsync(id, details);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
