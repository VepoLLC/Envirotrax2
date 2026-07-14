using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Sites;

[Route("api/sites/logs")]
[PermissionResource(PermissionType.CsiReports, PermissionType.BackflowReports, PermissionType.FogReports)]
public class PropertyLogManagementController : WaterSupplierProtectedController
{
    private readonly ISiteLogService _siteLogService;

    public PropertyLogManagementController(ISiteLogService siteLogService)
    {
        _siteLogService = siteLogService;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _siteLogService.GetForManagementAsync(pageInfo, query, cancellationToken);

        return Ok(result);
    }
}
