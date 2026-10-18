using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/testers/{professionalId}/water-suppliers")]
public class BackflowTesterWaterSupplierController : AdminBaseController
{
    private readonly IBackflowTesterWaterSupplierService _supplierService;

    public BackflowTesterWaterSupplierController(IBackflowTesterWaterSupplierService supplierService)
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
