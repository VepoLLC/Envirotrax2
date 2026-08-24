using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/permit-compliance")]
[HasFeature(FeatureType.FogInspection)]
[PermissionResource(PermissionType.FogReports)]
public class FogPermitComplianceController : WaterSupplierProtectedController
{
    private readonly ISiteService _siteService;

    public FogPermitComplianceController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetFogPermitComplianceAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _siteService.GetFogPermitComplianceAsync(pageInfo, query, cancellationToken);

        return Ok(result);
    }
}
