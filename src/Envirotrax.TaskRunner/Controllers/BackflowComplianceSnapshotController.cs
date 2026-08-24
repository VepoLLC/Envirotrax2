
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.TaskRunner.Controllers;

[ApiController]
[Route("api/backflow-compliance-snapshots")]
public class BackflowComplianceSnapshotController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly IInternalApiClientService _internalApi;

    public BackflowComplianceSnapshotController(
        IQueueService queueService,
        IInternalApiClientService internalApi)
    {
        _queueService = queueService;
        _internalApi = internalApi;
    }

    // Entry point for the external monthly scheduler: enumerate every active supplier and enqueue one
    // snapshot message per supplier for the current month. The worker fans each out to the App.
    [HttpPost("process")]
    public async Task<IActionResult> ProcessAsync(CancellationToken cancellationToken)
    {
        var supplierIds = await _internalApi.GetAsync<ICollection<int>>(
            null,
            "api/task-runner/water-suppliers",
            cancellationToken);

        var now = DateTime.UtcNow;
        var reportDate = new DateTime(now.Year, now.Month, 1);

        await _queueService.EnsureQueueExistsAsync(QueueNames.BackflowComplianceSnapshots.Process, cancellationToken);

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = cancellationToken };

        await Parallel.ForEachAsync(supplierIds!, parallelOptions, async (supplierId, ct) =>
        {
            if (!ct.IsCancellationRequested)
            {
                var message = new ComplianceSnapshotMessageDto
                {
                    WaterSupplier = new WaterSupplierDto { Id = supplierId },
                    ReportDate = reportDate
                };

                await _queueService.SendMessageAsync(QueueNames.BackflowComplianceSnapshots.Process, message, ct);
            }
        });

        return Accepted(supplierIds!.Count);
    }

    // One-time entry point to seed the full history for every active supplier. Enqueues one backfill
    // message per supplier; the backfill worker fans each out to the App, which reconstructs and stores
    // that supplier's history.
    [HttpPost("backfill")]
    public async Task<IActionResult> BackfillAsync(CancellationToken cancellationToken)
    {
        var supplierIds = await _internalApi.GetAsync<ICollection<int>>(
            null,
            "api/task-runner/water-suppliers",
            cancellationToken);

        await _queueService.EnsureQueueExistsAsync(QueueNames.BackflowComplianceSnapshots.Backfill, cancellationToken);

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = cancellationToken };

        await Parallel.ForEachAsync(supplierIds!, parallelOptions, async (supplierId, ct) =>
        {
            if (!ct.IsCancellationRequested)
            {
                var message = new WaterSupplierDto { Id = supplierId };

                await _queueService.SendMessageAsync(QueueNames.BackflowComplianceSnapshots.Backfill, message, ct);
            }
        });

        return Accepted(supplierIds!.Count);
    }
}
