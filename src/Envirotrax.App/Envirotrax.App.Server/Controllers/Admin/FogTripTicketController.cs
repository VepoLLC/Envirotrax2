using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/trip-tickets")]
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
        var tickets = await _tripTicketService.SearchForAdminAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(tickets);
    }
}
