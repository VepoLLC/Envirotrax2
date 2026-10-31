using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/inspectors/{professionalId}/water-suppliers")]
public class FogInspectorWaterSupplierController : AdminBaseController
{
    private readonly IFogInspectorWaterSupplierService _supplierService;

    public FogInspectorWaterSupplierController(IFogInspectorWaterSupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var registrations = await _supplierService.GetAllAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(registrations);
    }

    [HttpPut("{waterSupplierId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int waterSupplierId, [FromBody] ProfessionalWaterSupplierDto registration, CancellationToken cancellationToken)
    {
        var updated = await _supplierService.UpdateAsync(professionalId, waterSupplierId, registration, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }
}
