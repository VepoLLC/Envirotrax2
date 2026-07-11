
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

    [HttpPost("extend-date")]
    public async Task<IActionResult> ExtendDateAsync(CancellationToken cancellationToken)
    {
        var tests = await _internalApi.GetAsync<ICollection<BackflowTestDto>>(null, $"api/task-runner/backflow-tests/renewal/pending?batchSize={_options.BatchSize}", cancellationToken);

        await _queueService.EnsureQueueExistsAsync(QueueNames.BackflowTests.ExtendDate, cancellationToken);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 10,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(tests!, parallelOptions, async (test, cancellationToken) =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await _queueService.SendMessageAsync(QueueNames.BackflowTests.ExtendDate, test, cancellationToken);
            }
        });

        return Accepted(tests!.Count);
    }
}
