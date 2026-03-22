
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/water-suppliers")]
[PermissionResource(PermissionType.WaterSuppliers)]
public class WaterSupplierController : WaterSupplierCrudController<WaterSupplierDto>
{
    private readonly IWaterSupplierService _supplierService;

    public WaterSupplierController(IWaterSupplierService service)
        : base(service)
    {
        _supplierService = service;
    }

    [HttpGet("my/current")]
    public async Task<IActionResult> GetLoggedInSupplierAsync(CancellationToken cancellationToken)
    {
        var supplier = await _supplierService.GetLoggedInSupplierAsync(cancellationToken);
        return Ok(supplier);
    }
}

// We this controller because /api/water-suppliers/my endpoint requires the user to be logged in, but it doesn't require having WaterSupplier role.
// When that endpoint is called, the WaterSupplier role is not set yet. That is why we need a different authroize attribute.
[Authorize]
[ApiController]
[Route("api/water-suppliers")]
public class MyWaterSupplierController : ControllerBase
{
    private readonly IWaterSupplierService _supplierService;

    public MyWaterSupplierController(IWaterSupplierService service)
    {
        _supplierService = service;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetAllMyTenantsAsync(CancellationToken cancellationToken)
    {
        var tenants = await _supplierService.GetAllMySuppliersAsync(cancellationToken);
        return Ok(tenants);
    }
}