
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/transporters")]
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
