using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.TaskRunner;

[Route("api/task-runner/backflow-compliance-snapshots")]
public class BackflowComplianceSnapshotController : TaskRunnerBaseContoller
{
    private readonly IBackflowComplianceSnapshotService _snapshotService;

    public BackflowComplianceSnapshotController(IBackflowComplianceSnapshotService snapshotService)
    {
        _snapshotService = snapshotService;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateAsync([FromBody] GenerateComplianceSnapshotRequest request, CancellationToken cancellationToken)
    {
        await _snapshotService.GenerateSnapshotAsync(request.ReportDate, cancellationToken);

        return NoContent();
    }

    // One-time backfill of the full history for the header-resolved supplier. Tenant comes from the
    // Vp-Water-Supplier-Id header, so there is no body or supplier parameter.
    [HttpPost("backfill")]
    public async Task<IActionResult> BackfillAsync(CancellationToken cancellationToken)
    {
        await _snapshotService.BackfillAsync(cancellationToken);

        return NoContent();
    }
}
