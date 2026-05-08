
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Configuration;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Envirotrax.TaskRunner.Controllers;

[ApiController]
[Route("api/sites")]
public class SiteController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly IInternalApiClientService _internalApi;
    private readonly GeocodingOptions _geocodingOptions;

    public SiteController(
        IQueueService queueService,
        IInternalApiClientService internalApi,
        IOptions<GeocodingOptions> geocodingOptions)
    {
        _queueService = queueService;
        _internalApi = internalApi;
        _geocodingOptions = geocodingOptions.Value;
    }

    [HttpPost("geocode")]
    public async Task<IActionResult> GeocodeAsync(CancellationToken cancellationToken)
    {
        var sites = await _internalApi.GetAsync<ICollection<SiteDto>>(null, $"api/task-runner/sites/geocode/pending?batchSize={_geocodingOptions.BatchSize}");

        await _queueService.EnsureQueueExistsAsync(QueueNames.Sites.Geocode, cancellationToken);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 10,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(sites!, parallelOptions, async (site, cancellationToken) =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await _queueService.SendMessageAsync(QueueNames.Sites.Geocode, site, cancellationToken);
            }
        });

        return Ok(sites!.Count);
    }
}