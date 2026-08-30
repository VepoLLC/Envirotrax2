using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Sites;

[Route("api/sites/fog-trip-ticket-compliance")]
[HasFeature(FeatureType.FogTransportation)]
[PermissionResource(PermissionType.FogReports)]
public class FogComplianceController : WaterSupplierProtectedController
{
    private readonly ISiteService _siteService;

    public FogComplianceController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetFogTripTicketComplianceAsync(
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        [FromQuery] DateTime? dueDateFrom,
        [FromQuery] DateTime? dueDateTo,
        [FromQuery] bool sortDescending,
        CancellationToken cancellationToken)
    {
        var result = await _siteService.GetFogTripTicketComplianceAsync(pageInfo, query, dueDateFrom, dueDateTo, sortDescending, cancellationToken);

        return Ok(result);
    }
}
