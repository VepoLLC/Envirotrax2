using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Csi;

[Route("api/csi/inspectors")]
public class CsiInspectorLicenseController : AdminBaseController
{
    private readonly ICsiInspectorLicenseService _licenseService;

    public CsiInspectorLicenseController(ICsiInspectorLicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpGet("licenses/types")]
    public async Task<IActionResult> GetTypesAsync(CancellationToken cancellationToken)
    {
        var types = await _licenseService.GetTypesAsync(cancellationToken);

        return Ok(types);
    }

    [HttpGet("{professionalId}/licenses")]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var licenses = await _licenseService.GetAllAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(licenses);
    }

    [HttpPost("{professionalId}/licenses")]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalUserLicenseDto license, CancellationToken cancellationToken)
    {
        var added = await _licenseService.AddAsync(professionalId, license, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{professionalId}/licenses/{licenseId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int licenseId, [FromBody] ProfessionalUserLicenseDto license, CancellationToken cancellationToken)
    {
        var updated = await _licenseService.UpdateAsync(professionalId, licenseId, license, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{professionalId}/licenses/{licenseId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int licenseId, CancellationToken cancellationToken)
    {
        await _licenseService.DeleteAsync(professionalId, licenseId, cancellationToken);

        return Ok();
    }
}
