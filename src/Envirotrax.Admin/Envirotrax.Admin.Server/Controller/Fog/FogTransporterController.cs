
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/transporters")]
public class FogTransporterController : AdminBaseController
{
    private readonly IFogTransporterService _transporterService;

    public FogTransporterController(IFogTransporterService transporterService)
    {
        _transporterService = transporterService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] FogTransporterSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var transporters = await _transporterService.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(transporters);
    }
}
