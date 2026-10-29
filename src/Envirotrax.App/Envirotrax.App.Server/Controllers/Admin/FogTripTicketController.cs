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

    [HttpPut("{id}/validation-hold")]
    public async Task<IActionResult> UpdateValidationHoldAsync(int id, [FromBody] bool onHold, CancellationToken cancellationToken)
    {
        var ticket = await _tripTicketService.UpdateValidationHoldAsync(id, onHold, cancellationToken);

        return ticket == null ? NotFound() : Ok(ticket);
    }

    [HttpPut("{id}/needs-validation")]
    public async Task<IActionResult> UpdateNeedsValidationAsync(int id, [FromBody] bool needsValidation, CancellationToken cancellationToken)
    {
        var ticket = await _tripTicketService.UpdateNeedsValidationAsync(id, needsValidation, cancellationToken);

        return ticket == null ? NotFound() : Ok(ticket);
    }
}
