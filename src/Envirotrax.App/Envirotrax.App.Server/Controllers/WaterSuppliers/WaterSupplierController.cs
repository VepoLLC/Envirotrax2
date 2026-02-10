
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/water-suppliers")]
public class WaterSupplierController : CrudController<WaterSupplierDto>
{
    private readonly IWaterSupplierService _supplierService;

    public WaterSupplierController(IWaterSupplierService service)
        : base(service)
    {
        _supplierService = service;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetAllMyTenantsAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var tenants = await _supplierService.GetAllMySuppliersAsync(pageInfo, query, cancellationToken);
        return Ok(tenants);
    }
}