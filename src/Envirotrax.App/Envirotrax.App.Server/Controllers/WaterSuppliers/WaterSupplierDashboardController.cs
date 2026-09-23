using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/water-suppliers/dashboard")]
public class WaterSupplierDashboardController : WaterSupplierProtectedController
{
    private readonly IWaterSupplierDashboardService _dashboardService;

    public WaterSupplierDashboardController(IWaterSupplierDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStatsAsync(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetStatsAsync(cancellationToken);

        return Ok(stats);
    }


    [HttpGet("csi-submission-stats")]
    public async Task<IActionResult> GetCsiSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetCsiSubmissionStatsAsync(cancellationToken);

        return Ok(stats);
    }

    [HttpGet("backflow-submission-stats")]
    public async Task<IActionResult> GetBackflowSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetBackflowSubmissionStatsAsync(cancellationToken);

        return Ok(stats);
    }

    [HttpGet("backflow-compliance")]
    [HasFeature(FeatureType.BackflowTesting)]
    public async Task<IActionResult> GetBackflowComplianceAsync(CancellationToken cancellationToken)
    {
        var compliance = await _dashboardService.GetBackflowComplianceAsync(cancellationToken);

        return Ok(compliance);
    }

    [HttpGet("fog-inspection-submission-stats")]
    public async Task<IActionResult> GetFogInspectionSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetFogInspectionSubmissionStatsAsync(cancellationToken);

        return Ok(stats);
    }

    [HttpGet("fog-trip-ticket-submission-stats")]
    public async Task<IActionResult> GetFogTripTicketSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetFogTripTicketSubmissionStatsAsync(cancellationToken);

        return Ok(stats);
    }
}
