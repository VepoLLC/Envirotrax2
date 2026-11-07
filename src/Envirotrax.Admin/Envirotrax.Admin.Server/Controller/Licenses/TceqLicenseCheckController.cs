using Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Licenses;

[Route("api/licenses/{licenseId}/tceq-checks")]
public class TceqLicenseCheckController : AdminBaseController
{
    private readonly ILicenseService _licenseService;

    public TceqLicenseCheckController(ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int licenseId)
    {
        var tceqLicense = await _licenseService.CheckTceqAsync(licenseId);

        if (tceqLicense == null)
        {
            return NoContent();
        }

        return Ok(tceqLicense);
    }
}
