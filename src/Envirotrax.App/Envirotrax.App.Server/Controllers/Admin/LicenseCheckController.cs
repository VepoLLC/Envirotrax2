using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/licenses/{licenseId}/checks")]
public class LicenseCheckController : AdminBaseController
{
    private readonly IProfessionalUserLicenseService _licenseService;

    public LicenseCheckController(IProfessionalUserLicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int licenseId)
    {
        var tceqLicense = await _licenseService.CheckTceqLicenseAsync(licenseId);

        if (tceqLicense == null)
        {
            return NoContent();
        }

        return Ok(tceqLicense);
    }
}
