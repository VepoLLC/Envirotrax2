using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/insurances")]
public class InsuranceManagementController : WaterSupplierProtectedController
{
    private readonly IProfessionalInsuranceService _insuranceService;
    private readonly IAuthService _authService;

    public InsuranceManagementController(IProfessionalInsuranceService insuranceService, IAuthService authService)
    {
        _insuranceService = insuranceService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInsurancesAsync(
        [FromQuery] string? insuranceFilter,
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        CancellationToken cancellationToken)
    {
        if (!HasInsuranceAccess())
            return Forbid();

        var result = await _insuranceService.GetAllByWaterSupplierAsync(pageInfo, query, insuranceFilter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("counts")]
    public async Task<IActionResult> GetCountsAsync(CancellationToken cancellationToken)
    {
        if (!HasInsuranceAccess())
            return Forbid();

        var counts = await _insuranceService.GetCountsByWaterSupplierAsync(cancellationToken);
        return Ok(counts);
    }

    [HttpGet("{id}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        if (!HasInsuranceAccess())
            return Forbid();

        var url = await _insuranceService.GenerateFileUrlForWaterSupplierAsync(id, cancellationToken);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInsuranceAsync(int id, [FromBody] UpdateWaterSupplierInsuranceDto dto, CancellationToken cancellationToken)
    {
        if (!HasInsuranceAccess() || !HasModifyAccess())
            return Forbid();

        var result = await _insuranceService.UpdateForWaterSupplierAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInsuranceAsync(int id, CancellationToken cancellationToken)
    {
        if (!HasInsuranceAccess() || !HasModifyAccess())
            return Forbid();

        await _insuranceService.DeleteForWaterSupplierAsync(id, cancellationToken);
        return NoContent();
    }

    private bool HasInsuranceAccess()
    {
        return _authService.HasAnyFeatures(FeatureType.ManageProfessionalInsurances) ||
               _authService.HasAnyPermission(PermissionAction.CanView, PermissionType.Licenses);
    }

    private bool HasModifyAccess()
    {
        return _authService.HasAnyFeatures(FeatureType.ManageProfessionalInsurances) ||
               _authService.HasAnyPermission(PermissionAction.CanModify, PermissionType.Licenses);
    }
}
