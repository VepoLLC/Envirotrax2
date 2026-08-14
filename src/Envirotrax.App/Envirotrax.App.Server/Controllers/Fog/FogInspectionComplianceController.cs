using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

// FOG Inspection Compliance Management: a paged list of sites whose FOG inspection is overdue, each with
// its property logs. Site-rooted, mirroring CsiComplianceController.
[Route("api/fog/inspection-compliance")]
[HasFeature(FeatureType.FogInspection)]
[PermissionResource(PermissionType.FogReports)]
public class FogInspectionComplianceController : WaterSupplierProtectedController
{
    private readonly ISiteService _siteService;

    public FogInspectionComplianceController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetFogInspectionComplianceAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _siteService.GetFogInspectionComplianceAsync(pageInfo, query, cancellationToken);

        return Ok(result);
    }
}
