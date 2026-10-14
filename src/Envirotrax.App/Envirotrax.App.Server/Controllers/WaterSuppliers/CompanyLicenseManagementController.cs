using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/company-licenses")]
public class CompanyLicenseManagementController : WaterSupplierProtectedController
{
    private readonly IProfessionalLicenseService _licenseService;
    private readonly IAuthService _authService;

    public CompanyLicenseManagementController(IProfessionalLicenseService licenseService, IAuthService authService)
    {
        _licenseService = licenseService;
        _authService = authService;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLicenseAsync(int id, [FromBody] UpdateWaterSupplierLicenseDto dto)
    {
        if (!HasModifyAccess())
        {
            return Forbid();
        }

        var result = await _licenseService.UpdateForWaterSupplierAsync(id, dto);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLicenseAsync(int id)
    {
        if (!HasModifyAccess())
        {
            return Forbid();
        }

        await _licenseService.DeleteForWaterSupplierAsync(id);

        return NoContent();
    }

    private bool HasModifyAccess()
    {
        return _authService.HasAnyFeatures(FeatureType.ManageProfessionalLicenses) ||
               _authService.HasAnyPermission(PermissionAction.CanModify, PermissionType.Licenses);
    }
}
