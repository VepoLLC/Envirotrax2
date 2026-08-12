using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/reports")]
[PermissionResource(PermissionType.FogReports)]
public class FogReportsController : WaterSupplierProtectedController
{
    private readonly IFogSystemReportService _systemReportService;

    public FogReportsController(IFogSystemReportService systemReportService)
    {
        _systemReportService = systemReportService;
    }

    [HttpGet("trip-tickets")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetTripTicketReportAsync([FromQuery] FogTripTicketReportDateType dateType, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, CancellationToken cancellationToken)
    {
        var report = await _systemReportService.GetTripTicketReportAsync(dateType, fromDate, toDate, cancellationToken);
        return Ok(report);
    }

    [HttpGet("trip-tickets/earliest-date")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetEarliestTripTicketDateAsync(CancellationToken cancellationToken)
    {
        var earliestDate = await _systemReportService.GetEarliestTripTicketDateAsync(cancellationToken);
        return Ok(new { earliestDate });
    }

    [HttpGet("inspections")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetInspectionReportAsync([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, CancellationToken cancellationToken)
    {
        var report = await _systemReportService.GetInspectionReportAsync(fromDate, toDate, cancellationToken);
        return Ok(report);
    }

    [HttpGet("inspections/earliest-date")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetEarliestInspectionDateAsync(CancellationToken cancellationToken)
    {
        var earliestDate = await _systemReportService.GetEarliestInspectionDateAsync(cancellationToken);
        return Ok(new { earliestDate });
    }
}
