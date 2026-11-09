using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/licenses")]
public class LicenseController : AdminBaseController
{
    private readonly IWaterSupplierLicenseService _waterSupplierLicenseService;
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IProfessionalLicenseTypeService _licenseTypeService;

    public LicenseController(
        IWaterSupplierLicenseService waterSupplierLicenseService,
        IProfessionalUserLicenseService licenseService,
        IProfessionalLicenseTypeService licenseTypeService)
    {
        _waterSupplierLicenseService = waterSupplierLicenseService;
        _licenseService = licenseService;
        _licenseTypeService = licenseTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] string? licenseFilter, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var licenses = await _waterSupplierLicenseService.GetAllForAdminAsync(licenseFilter, pageInfo, query, cancellationToken);

        return Ok(licenses);
    }

    [HttpGet("counts")]
    public async Task<IActionResult> GetCountsAsync(CancellationToken cancellationToken)
    {
        var counts = await _waterSupplierLicenseService.GetCountsForAdminAsync(cancellationToken);

        return Ok(counts);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetTypesAsync([FromQuery] Query query, CancellationToken cancellationToken)
    {
        var types = await _licenseTypeService.GetAllAsync(query, cancellationToken);

        return Ok(types);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProfessionalUserLicenseDto license)
    {
        var updated = await _licenseService.UpdateForAdminAsync(id, license);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
