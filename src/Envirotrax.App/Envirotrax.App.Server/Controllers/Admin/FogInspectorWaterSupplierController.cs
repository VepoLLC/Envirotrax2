using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/inspectors/{professionalId}/water-suppliers")]
public class FogInspectorWaterSupplierController : AdminBaseController
{
    private static readonly Expression<Func<ProfessionalWaterSupplier, bool>> FogInspectorRegistrations =
        proSupplier => proSupplier.HasFogInspection || proSupplier.WaterSupplier!.GeneralSettings!.FogProgram;

    private readonly IProfessionalSupplierService _supplierService;

    public FogInspectorWaterSupplierController(IProfessionalSupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var registrations = await _supplierService.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken, FogInspectorRegistrations);

        return Ok(registrations);
    }

    [HttpPut("{waterSupplierId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int waterSupplierId, [FromBody] ProfessionalWaterSupplierDto registration, CancellationToken cancellationToken)
    {
        if (!await IsRegisteredAsync(professionalId, waterSupplierId, cancellationToken))
        {
            return NotFound();
        }

        registration.Professional = new ReferencedProfessionalDto
        {
            Id = professionalId
        };

        registration.WaterSupplier = new ReferencedWaterSupplierDto
        {
            Id = waterSupplierId
        };

        var updated = await _supplierService.UpdateAsync(registration);

        return Ok(updated);
    }

    private async Task<bool> IsRegisteredAsync(int professionalId, int waterSupplierId, CancellationToken cancellationToken)
    {
        var registrations = await _supplierService.GetAllByProfessionalAsync(
            professionalId,
            new PageInfo(),
            new Query(),
            cancellationToken,
            proSupplier => proSupplier.WaterSupplierId == waterSupplierId);

        return registrations.Data.Any();
    }
}
