
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/water-suppliers")]
[PermissionResource(PermissionType.WaterSuppliers)]
public class WaterSupplierController : CrudController<WaterSupplierDto>
{
    private readonly IWaterSupplierService _supplierService;

    public WaterSupplierController(IWaterSupplierService service)
        : base(service)
    {
        _supplierService = service;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetAllMyTenantsAsync(CancellationToken cancellationToken)
    {
        var tenants = await _supplierService.GetAllMySuppliersAsync(cancellationToken);
        return Ok(tenants);
    }

    [HttpGet("my/current")]
    public async Task<IActionResult> GetLoggedInSupplierAsync(CancellationToken cancellationToken)
    {
        var supplier = await _supplierService.GetLoggedInSupplierAsync(cancellationToken);
        return Ok(supplier);
    }
}