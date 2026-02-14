
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Controllers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

[Route("api/professionals/water-suppliers")]
public class ProfessionalSupplierContoller : ProtectedController
{
    private readonly IProfessionalSupplierService _proSupplierService;

    public ProfessionalSupplierContoller(IProfessionalSupplierService proSupplierService)
    {
        _proSupplierService = proSupplierService;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var suppliers = await _proSupplierService.GetAllAvailableSuppliersAsync(pageInfo, query, cancellationToken);
        return Ok(suppliers);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var proSuppliers = await _proSupplierService.GetAllAsync(cancellationToken);
        return Ok(proSuppliers);
    }

    [HttpPut("my/add-or-update")]
    public async Task<IActionResult> AddOrUpdateAsync(ProfessionalWaterSupplierDto proSupplier)
    {
        var updated = await _proSupplierService.AddOrUpdateAsync(proSupplier);
        return Ok(updated);
    }
}