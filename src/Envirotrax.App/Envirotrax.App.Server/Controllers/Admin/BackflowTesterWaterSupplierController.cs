using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/testers/{professionalId}/water-suppliers")]
public class BackflowTesterWaterSupplierController : AdminBaseController
{
    // V1 listed the BPAT registrations (SaveWaterSupplierRegistrations.UserType = BPAT). Filtering on HasBackflowTesting
    // alone would make the Active checkbox one-way - unticking it would remove the row and leave no way back - so keep
    // every supplier that runs a backflow program too, the same rule the admin CSI window applies to CSI.
    private static readonly Expression<Func<ProfessionalWaterSupplier, bool>> BackflowRegistrations =
        proSupplier => proSupplier.HasBackflowTesting || proSupplier.WaterSupplier!.GeneralSettings!.BackflowTesting;

    private readonly IProfessionalSupplierService _supplierService;

    public BackflowTesterWaterSupplierController(IProfessionalSupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var registrations = await _supplierService.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken, BackflowRegistrations);

        return Ok(registrations);
    }

    [HttpPut("{waterSupplierId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int waterSupplierId, [FromBody] ProfessionalWaterSupplierDto registration, CancellationToken cancellationToken)
    {
        // The update overwrites the whole row, so it may only touch a registration this professional already holds.
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
