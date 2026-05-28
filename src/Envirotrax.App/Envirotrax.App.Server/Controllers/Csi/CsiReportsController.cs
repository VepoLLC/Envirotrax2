using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi;

[Route("api/csi/reports")]
[HasFeature(FeatureType.CsiInspection)]
[PermissionResource(PermissionType.CsiReports)]
public class CsiReportsController : WaterSupplierProtectedController
{
    private readonly ICsiSystemReportService _systemReportService;

    public CsiReportsController(ICsiSystemReportService systemReportService)
    {
        _systemReportService = systemReportService;
    }

    [HttpGet("system")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetSystemReportAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var report = await _systemReportService.GetSystemReportAsync(fromDate, toDate, cancellationToken);
        return Ok(report);
    }
}
