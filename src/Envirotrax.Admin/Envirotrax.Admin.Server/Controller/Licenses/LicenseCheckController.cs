using Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Licenses;

[Route("api/licenses/{licenseId}/checks")]
public class LicenseCheckController : AdminBaseController
{
    private readonly ILicenseService _licenseService;

    public LicenseCheckController(ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int licenseId)
    {
        var tceqLicense = await _licenseService.CheckAsync(licenseId);

        if (tceqLicense == null)
        {
            return NoContent();
        }

        return Ok(tceqLicense);
    }
}
