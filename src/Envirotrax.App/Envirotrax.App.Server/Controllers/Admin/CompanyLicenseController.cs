using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/company-licenses")]
public class CompanyLicenseController : AdminBaseController
{
    private readonly IProfessionalLicenseService _licenseService;

    public CompanyLicenseController(IProfessionalLicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProfessionalLicenseDto license)
    {
        var updated = await _licenseService.UpdateForAdminAsync(id, license);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
