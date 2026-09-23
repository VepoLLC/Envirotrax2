using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/csi/inspectors")]
public class CsiInspectorLicenseController : AdminBaseController
{
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IProfessionalLicenseTypeService _licenseTypeService;

    public CsiInspectorLicenseController(
        IProfessionalUserLicenseService licenseService,
        IProfessionalLicenseTypeService licenseTypeService)
    {
        _licenseService = licenseService;
        _licenseTypeService = licenseTypeService;
    }

    [HttpGet("licenses/types")]
    public async Task<IActionResult> GetTypesAsync([FromQuery] Query query, CancellationToken cancellationToken)
    {
        var types = await _licenseTypeService.GetAllAsync(query, cancellationToken);

        return Ok(types.Where(type => type.ProfessionalType == ProfessionalType.CsiInspector));
    }

    [HttpGet("{professionalId}/licenses")]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var licenses = await _licenseService.GetAllByProfessionalAsync(
            professionalId,
            pageInfo,
            query,
            cancellationToken,
            license => license.ProfessionalType == ProfessionalType.CsiInspector);

        return Ok(licenses);
    }

    [HttpPost("{professionalId}/licenses")]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalUserLicenseDto license)
    {
        // The grid is scoped to CSI licenses, so anything added here is a CSI license by definition.
        license.ProfessionalType = ProfessionalType.CsiInspector;

        var added = await _licenseService.AddForProfessionalAsync(professionalId, license);

        return Ok(added);
    }

    [HttpPut("{professionalId}/licenses/{licenseId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int licenseId, [FromBody] ProfessionalUserLicenseDto license, CancellationToken cancellationToken)
    {
        // UpdateForProfessionalAsync overwrites the whole row and stamps ProfessionalId from the
        // argument, so without this check a PUT could re-parent and re-type another professional's
        // BPAT or FOG license.
        if (!await BelongsToProfessionalAsync(professionalId, licenseId, cancellationToken))
        {
            return NotFound();
        }

        license.Id = licenseId;
        license.ProfessionalType = ProfessionalType.CsiInspector;

        var updated = await _licenseService.UpdateForProfessionalAsync(professionalId, license);

        return Ok(updated);
    }

    [HttpDelete("{professionalId}/licenses/{licenseId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int licenseId, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, licenseId, cancellationToken))
        {
            return NotFound();
        }

        var deleted = await _licenseService.DeleteAsync(licenseId);

        return deleted == null ? NotFound() : Ok();
    }

    private async Task<bool> BelongsToProfessionalAsync(int professionalId, int licenseId, CancellationToken cancellationToken)
    {
        var owned = await _licenseService.GetAllByProfessionalAsync(
            professionalId,
            new PageInfo(),
            new Query(),
            cancellationToken,
            license => license.Id == licenseId && license.ProfessionalType == ProfessionalType.CsiInspector);

        return owned.Data.Any();
    }
}
