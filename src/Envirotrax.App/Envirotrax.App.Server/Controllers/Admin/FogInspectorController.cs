
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// Admin FOG inspector search. A route shell only - it reuses the existing IFogInspectorService.SearchAsync that the
/// water-supplier window uses. That endpoint (api/fog/inspectors/search) is unreachable by an admin caller
/// (different scope, role and feature gate), so the admin surface needs its own route, but not its own search.
/// IDbContextSelector already routes admin requests to AdminDbContext, which disables tenant filtering.
/// </summary>
[Route("api/admin/fog/inspectors")]
public class FogInspectorController : AdminBaseController
{
    private readonly IFogInspectorService _inspectorService;

    public FogInspectorController(IFogInspectorService inspectorService)
    {
        _inspectorService = inspectorService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] FogInspectorSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var inspectors = await _inspectorService.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(inspectors);
    }
}
