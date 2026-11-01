using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/inspectors")]
public class FogInspectorLicenseController : AdminBaseController
{
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IProfessionalLicenseTypeService _licenseTypeService;

    public FogInspectorLicenseController(
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

        return Ok(types.Where(type => type.ProfessionalType == ProfessionalType.FogInspector && type.LicenseScope == LicenseScope.User));
    }

    [HttpGet("{professionalId}/licenses")]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var licenses = await _licenseService.GetAllByProfessionalAsync(
            professionalId,
            pageInfo,
            query,
            cancellationToken,
            license => license.ProfessionalType == ProfessionalType.FogInspector);

        return Ok(licenses);
    }

    [HttpPost("{professionalId}/licenses")]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalUserLicenseDto license)
    {
        license.ProfessionalType = ProfessionalType.FogInspector;

        var added = await _licenseService.AddForProfessionalAsync(professionalId, license);

        return Ok(added);
    }

    [HttpPut("{professionalId}/licenses/{licenseId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int licenseId, [FromBody] ProfessionalUserLicenseDto license, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, licenseId, cancellationToken))
        {
            return NotFound();
        }

        license.Id = licenseId;
        license.ProfessionalType = ProfessionalType.FogInspector;

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
            license => license.Id == licenseId && license.ProfessionalType == ProfessionalType.FogInspector);

        return owned.Data.Any();
    }
}
