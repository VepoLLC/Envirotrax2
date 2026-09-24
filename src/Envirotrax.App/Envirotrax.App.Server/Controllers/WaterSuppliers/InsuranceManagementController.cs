using DeveloperPartners.SortingFiltering;
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
    public async Task<IActionResult> GetUnverifiedInsurancesAsync(
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        CancellationToken cancellationToken)
    {
        if (!HasInsuranceAccess())
        {
            return Forbid();
        }

        var result = await _insuranceService.GetUnverifiedByWaterSupplierAsync(pageInfo, query, cancellationToken);

        return Ok(result);
    }

    private bool HasInsuranceAccess()
    {
        return _authService.HasAnyFeatures(FeatureType.ManageProfessionalInsurances) ||
               _authService.HasAnyPermission(PermissionAction.CanView, PermissionType.Licenses);
    }
}
