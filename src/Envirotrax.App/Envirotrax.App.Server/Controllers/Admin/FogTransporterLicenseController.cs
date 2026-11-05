using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// FOG transporter registrations are company-level licenses (ProfessionalLicenses, LicenseScope.Company), unlike
/// the FOG inspector's user-level ones, so this goes through IProfessionalLicenseService like the water-supplier
/// transporter window does.
/// </summary>
[Route("api/admin/fog/transporters")]
public class FogTransporterLicenseController : AdminBaseController
{
    private readonly IProfessionalLicenseService _licenseService;
    private readonly IProfessionalLicenseTypeService _licenseTypeService;

    public FogTransporterLicenseController(
        IProfessionalLicenseService licenseService,
        IProfessionalLicenseTypeService licenseTypeService)
    {
        _licenseService = licenseService;
        _licenseTypeService = licenseTypeService;
    }

    [HttpGet("licenses/types")]
    public async Task<IActionResult> GetTypesAsync([FromQuery] Query query, CancellationToken cancellationToken)
    {
        var types = await _licenseTypeService.GetAllAsync(query, cancellationToken);

        return Ok(types.Where(type => type.ProfessionalType == ProfessionalType.FogTransporter && type.LicenseScope == LicenseScope.Company));
    }

    [HttpGet("{professionalId}/licenses")]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var licenses = await _licenseService.GetAllByProfessionalAsync(professionalId, ProfessionalType.FogTransporter, pageInfo, query, cancellationToken);

        return Ok(licenses);
    }

    [HttpPost("{professionalId}/licenses")]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalLicenseDto license)
    {
        var added = await _licenseService.AddForProfessionalAsync(professionalId, license);

        return Ok(added);
    }

    [HttpPut("{professionalId}/licenses/{licenseId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int licenseId, [FromBody] ProfessionalLicenseDto license)
    {
        license.Id = licenseId;

        var updated = await _licenseService.UpdateForProfessionalAsync(professionalId, license);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{professionalId}/licenses/{licenseId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int licenseId, CancellationToken cancellationToken)
    {
        var deleted = await _licenseService.DeleteForProfessionalAsync(professionalId, licenseId, cancellationToken);

        return deleted == null ? NotFound() : Ok();
    }
}
