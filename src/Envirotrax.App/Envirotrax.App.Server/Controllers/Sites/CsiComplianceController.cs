using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Sites;

[Route("api/sites/csi-compliance")]
[HasFeature(FeatureType.CsiInspection)]
[PermissionResource(PermissionType.CsiReports)]
public class CsiComplianceController : WaterSupplierProtectedController
{
    private readonly ISiteService _siteService;

    public CsiComplianceController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetCsiComplianceAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _siteService.GetCsiComplianceAsync(pageInfo, query, cancellationToken);

        return Ok(result);
    }
}
