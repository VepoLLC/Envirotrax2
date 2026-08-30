using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/trip-tickets")]
[PermissionResource(PermissionType.FogTripTickets)]
public class FogTripTicketController : WaterSupplierCrudController<FogTripTicketDto>
{
    private readonly IFogTripTicketService _tripTicketService;

    public FogTripTicketController(IFogTripTicketService service)
        : base(service)
    {
        _tripTicketService = service;
    }

    protected override Task<IPagedData<FogTripTicketDto>> ProcessGetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var subAccountWaterSupplierId = ReadSubAccountWaterSupplierId();

        if (subAccountWaterSupplierId.HasValue)
        {
            return _tripTicketService.SearchForSubAccountAsync(pageInfo, query, subAccountWaterSupplierId.Value, cancellationToken);
        }

        return base.ProcessGetAllAsync(pageInfo, query, cancellationToken);
    }

    // Dashboard "View" drill-down into a sub account's trip tickets, without switching the current
    // session's authentication. Only water suppliers that are an actual child of the current
    // tenant are honored - see FogTripTicketRepository.SearchForSubAccountAsync.
    private int? ReadSubAccountWaterSupplierId()
    {
        if (int.TryParse(Request.Query["subAccountWaterSupplierId"], out var waterSupplierId))
        {
            return waterSupplierId;
        }

        return null;
    }

    [HttpPut("{id}/approval")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateApprovalAsync(int id, [FromBody] bool disapproved, CancellationToken cancellationToken)
    {
        var result = await _tripTicketService.UpdateApprovalAsync(id, disapproved, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}/pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetPdfAsync(int id, CancellationToken cancellationToken)
    {
        var ticket = await _tripTicketService.GetAsync(id, cancellationToken);

        if (ticket == null)
        {
            return NotFound();
        }

        var pdfBytes = await _tripTicketService.GeneratePdfAsync(ticket);
        return File(pdfBytes, "application/pdf");
    }
}
