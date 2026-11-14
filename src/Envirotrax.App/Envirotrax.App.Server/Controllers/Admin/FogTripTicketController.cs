using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/trip-tickets")]
public class FogTripTicketController : AdminBaseController
{
    private readonly IFogTripTicketService _tripTicketService;
    private readonly IRecordLogService _recordLogService;

    public FogTripTicketController(IFogTripTicketService tripTicketService, IRecordLogService recordLogService)
    {
        _tripTicketService = tripTicketService;
        _recordLogService = recordLogService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] FogTripTicketSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var tickets = await _tripTicketService.SearchForAdminAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var ticket = await _tripTicketService.GetAsync(id, cancellationToken);

        return ticket == null ? NotFound() : Ok(ticket);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] FogTripTicketAdminUpdateRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _tripTicketService.UpdateForAdminAsync(id, request, cancellationToken);

        return ticket == null ? NotFound() : Ok(ticket);
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

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _recordLogService.GetByRecordAsync(RecordLogTableNames.FogTripTickets, id, cancellationToken);

        return Ok(logs);
    }
}
