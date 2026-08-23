
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

    [HttpPost("process")]
    public async Task<IActionResult> ProcessAsync(CancellationToken cancellationToken)
    {
        var supplierIds = await _internalApi.GetAsync<ICollection<int>>(
            null,
            "api/task-runner/water-suppliers",
            cancellationToken);

        var reportDate = DateTime.UtcNow.Date;

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
}
