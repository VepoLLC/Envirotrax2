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

    [HttpPut("{id}/validation-hold")]
    public async Task<IActionResult> UpdateValidationHoldAsync(int id, [FromQuery] int waterSupplierId, [FromBody] bool onHold, CancellationToken cancellationToken)
    {
        var ticket = await _tripTicketService.UpdateValidationHoldAsync(id, waterSupplierId, onHold, cancellationToken);

        return ticket == null ? NotFound() : Ok(ticket);
    }

    [HttpPut("{id}/needs-validation")]
    public async Task<IActionResult> UpdateNeedsValidationAsync(int id, [FromQuery] int waterSupplierId, [FromBody] bool needsValidation, CancellationToken cancellationToken)
    {
        var ticket = await _tripTicketService.UpdateNeedsValidationAsync(id, waterSupplierId, needsValidation, cancellationToken);

        return ticket == null ? NotFound() : Ok(ticket);
    }
}
