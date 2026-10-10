
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/inspectors")]
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
