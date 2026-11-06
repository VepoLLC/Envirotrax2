using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Licenses;

[Route("api/licenses")]
public class LicenseController : AdminBaseController
{
    private readonly ILicenseService _licenseService;

    public LicenseController(ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] string? licenseFilter, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var licenses = await _licenseService.GetAllAsync(licenseFilter, pageInfo, query, cancellationToken);

        return Ok(licenses);
    }

    [HttpGet("counts")]
    public async Task<IActionResult> GetCountsAsync(CancellationToken cancellationToken)
    {
        var counts = await _licenseService.GetCountsAsync(cancellationToken);

        return Ok(counts);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetTypesAsync(CancellationToken cancellationToken)
    {
        var types = await _licenseService.GetTypesAsync(cancellationToken);

        return Ok(types);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProfessionalUserLicenseDto license)
    {
        var updated = await _licenseService.UpdateAsync(id, license);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
