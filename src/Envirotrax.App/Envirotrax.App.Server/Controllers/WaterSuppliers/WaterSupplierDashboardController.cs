using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
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
}
