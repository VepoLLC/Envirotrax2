
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Configuration;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Envirotrax.TaskRunner.Controllers;

[ApiController]
[Route("api/backflow-tests")]
public class BackflowTestController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly IInternalApiClientService _internalApi;
    private readonly BackflowRenewalOptions _options;

    public BackflowTestController(
        IQueueService queueService,
        IInternalApiClientService internalApi,
        IOptions<BackflowRenewalOptions> options)
    {
        _queueService = queueService;
        _internalApi = internalApi;
        _options = options.Value;
    }

    [HttpPost("process-tests-renewal")]
    public async Task<IActionResult> ProcessTestsRenewalAsync(CancellationToken cancellationToken)
    {
        var tests = await _internalApi.GetAsync<ICollection<BackflowTestDto>>(
            null,
            $"api/task-runner/backflow-tests/renewal/pending-tests?batchSize={_options.BatchSize}",
            cancellationToken);

        await _queueService.EnsureQueueExistsAsync(QueueNames.BackflowTests.ProcessTestRenewal, cancellationToken);

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = cancellationToken };

        await Parallel.ForEachAsync(tests!, parallelOptions, async (test, cancellationToken) =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await _queueService.SendMessageAsync(QueueNames.BackflowTests.ProcessTestRenewal, test, cancellationToken);
            }
        });

        return Accepted(tests!.Count);
    }

    [HttpPost("compliance-snapshots")]
    public async Task<IActionResult> ProcessComplianceSnapshotsAsync(CancellationToken cancellationToken)
    {
        var supplierIds = await _internalApi.GetAsync<ICollection<int>>(
            null,
            "api/task-runner/water-suppliers?hasBackflowTests=true",
            cancellationToken);

        var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        var reportDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralTimeZone).Date;

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
