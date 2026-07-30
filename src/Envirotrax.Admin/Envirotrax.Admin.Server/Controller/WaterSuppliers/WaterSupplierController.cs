

using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.Admin.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.WaterSuppliers;

[Route("api/water-suppliers")]
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

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetailsAsync(int id, CancellationToken cancellationToken)
    {
        var details = await _waterSupplierService.GetDetailsAsync(id, cancellationToken);

        if (details == null)
        {
            return NotFound();
        }

        return Ok(details);
    }

    [HttpGet("{id}/user-accounts")]
    public async Task<IActionResult> GetUserAccountsAsync(int id, CancellationToken cancellationToken)
    {
        var accounts = await _waterSupplierService.GetUserAccountsAsync(id, cancellationToken);
        return Ok(accounts);
    }

    [HttpPut("{id}/details")]
    public async Task<IActionResult> UpdateDetailsAsync(int id, WaterSupplierDetailsDto details)
    {
        var updated = await _waterSupplierService.UpdateDetailsAsync(id, details);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
