using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi;

[Route("api/csi/reports/system")]
[HasFeature(FeatureType.CsiInspection)]
[PermissionResource(PermissionType.CsiReports)]
public class CsiSystemReportController(ICsiSystemReportService reportService) : WaterSupplierProtectedController
{
    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var report = await reportService.GetSystemReportAsync(fromDate, toDate, cancellationToken);
        return Ok(report);
    }
}
