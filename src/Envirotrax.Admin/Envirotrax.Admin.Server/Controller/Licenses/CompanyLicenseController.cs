using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Licenses;

[Route("api/company-licenses")]
public class CompanyLicenseController : AdminBaseController
{
    private readonly ILicenseService _licenseService;

    public CompanyLicenseController(ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProfessionalLicenseDto license)
    {
        var updated = await _licenseService.UpdateCompanyLicenseAsync(id, license);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
