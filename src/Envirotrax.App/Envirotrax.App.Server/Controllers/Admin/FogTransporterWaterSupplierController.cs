using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/transporters/{professionalId}/water-suppliers")]
public class FogTransporterWaterSupplierController : AdminBaseController
{
    // FogProgram covers both FOG inspection and transportation, so suppliers running it are listed even before the
    // transporter is activated there.
    private static readonly Expression<Func<ProfessionalWaterSupplier, bool>> FogTransporterRegistrations =
        proSupplier => proSupplier.HasFogTransportation || proSupplier.WaterSupplier!.GeneralSettings!.FogProgram;

    private readonly IProfessionalSupplierService _supplierService;

    public FogTransporterWaterSupplierController(IProfessionalSupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var registrations = await _supplierService.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken, FogTransporterRegistrations);

        return Ok(registrations);
    }

    [HttpPut("{waterSupplierId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int waterSupplierId, [FromBody] ProfessionalWaterSupplierDto registration, CancellationToken cancellationToken)
    {
        var updated = await _supplierService.UpdateForProfessionalAsync(professionalId, waterSupplierId, registration, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }
}
