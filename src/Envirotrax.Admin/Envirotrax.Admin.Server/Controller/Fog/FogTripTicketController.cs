using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/trip-tickets")]
public class FogTripTicketController : AdminBaseController
{
    private readonly IFogTripTicketService _tripTicketService;

    public FogTripTicketController(IFogTripTicketService tripTicketService)
    {
        _tripTicketService = tripTicketService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] FogTripTicketSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var tickets = await _tripTicketService.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(tickets);
    }
}
