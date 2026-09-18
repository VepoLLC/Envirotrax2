using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/gauges")]
public class GaugeManagementController : WaterSupplierProtectedController
{
    private readonly IBackflowGaugeService _gaugeService;
    private readonly IAuthService _authService;

    public GaugeManagementController(IBackflowGaugeService gaugeService, IAuthService authService)
    {
        _gaugeService = gaugeService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGaugesAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        if (!HasGaugeAccess())
        {
            return Forbid();
        }

        var result = await _gaugeService.GetUnverifiedByWaterSupplierAsync(pageInfo, query, cancellationToken);
        return Ok(result);
    }

    // Reuses the licensing permission, as V1 did: there is no separate gauge-management feature flag.
    private bool HasGaugeAccess()
    {
        return _authService.HasAnyFeatures(FeatureType.ManageProfessionalLicenses) ||
               _authService.HasAnyPermission(PermissionAction.CanView, PermissionType.Licenses);
    }
}
