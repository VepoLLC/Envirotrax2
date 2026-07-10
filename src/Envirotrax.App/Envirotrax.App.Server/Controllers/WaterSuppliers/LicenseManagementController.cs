using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/licenses")]
public class LicenseManagementController : WaterSupplierProtectedController
{
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IAuthService _authService;

    public LicenseManagementController(IProfessionalUserLicenseService licenseService, IAuthService authService)
    {
        _licenseService = licenseService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLicensesAsync(
        [FromQuery] string? licenseFilter,
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        CancellationToken cancellationToken)
    {
        if (!HasLicenseAccess())
            return Forbid();

        var result = await _licenseService.GetAllByWaterSupplierAsync(pageInfo, query, licenseFilter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("counts")]
    public async Task<IActionResult> GetCountsAsync(CancellationToken cancellationToken)
    {
        if (!HasLicenseAccess())
            return Forbid();

        var counts = await _licenseService.GetCountsByWaterSupplierAsync(cancellationToken);
        return Ok(counts);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLicenseAsync(int id, [FromBody] UpdateWaterSupplierLicenseDto dto, CancellationToken cancellationToken)
    {
        if (!HasLicenseAccess() || !HasModifyAccess())
            return Forbid();

        var result = await _licenseService.UpdateForWaterSupplierAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLicenseAsync(int id, CancellationToken cancellationToken)
    {
        if (!HasLicenseAccess() || !HasModifyAccess())
            return Forbid();

        await _licenseService.DeleteForWaterSupplierAsync(id, cancellationToken);
        return NoContent();
    }

    private bool HasLicenseAccess()
    {
        return _authService.HasAnyFeatures(FeatureType.ManageProfessionalLicenses) ||
               _authService.HasAnyPermission(PermissionAction.CanView, PermissionType.Licenses);
    }

    private bool HasModifyAccess()
    {
        return _authService.HasAnyFeatures(FeatureType.ManageProfessionalLicenses) ||
               _authService.HasAnyPermission(PermissionAction.CanModify, PermissionType.Licenses);
    }
}
