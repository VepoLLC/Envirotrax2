

using DeveloperPartners.SortingFiltering;
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
}