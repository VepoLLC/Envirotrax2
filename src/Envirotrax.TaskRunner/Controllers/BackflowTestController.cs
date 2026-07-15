
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

    [HttpPost("process-sites-renewal")]
    public async Task<IActionResult> ProcessSitesRenewalAsync(CancellationToken cancellationToken)
    {
        var sites = await _internalApi.GetAsync<ICollection<SiteDto>>(
            null,
            $"api/task-runner/backflow-tests/renewal/pending-sites?batchSize={_options.BatchSize}",
            cancellationToken);

        await _queueService.EnsureQueueExistsAsync(QueueNames.BackflowTests.ProcessSiteRenewal, cancellationToken);

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = cancellationToken };
        await Parallel.ForEachAsync(sites!, parallelOptions, async (site, ct) =>
        {
            if (!ct.IsCancellationRequested)
                await _queueService.SendMessageAsync(QueueNames.BackflowTests.ProcessSiteRenewal, site, ct);
        });

        return Accepted(sites!.Count);
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
        await Parallel.ForEachAsync(tests!, parallelOptions, async (test, ct) =>
        {
            if (!ct.IsCancellationRequested)
                await _queueService.SendMessageAsync(QueueNames.BackflowTests.ProcessTestRenewal, test, ct);
        });

        return Accepted(tests!.Count);
    }
}
