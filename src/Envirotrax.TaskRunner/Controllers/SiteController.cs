
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
    private readonly BackflowRenewalOptions _renewalOptions;

    public SiteController(
        IQueueService queueService,
        IInternalApiClientService internalApi,
        IOptions<GeocodingOptions> geocodingOptions,
        IOptions<BackflowRenewalOptions> renewalOptions)
    {
        _queueService = queueService;
        _internalApi = internalApi;
        _geocodingOptions = geocodingOptions.Value;
        _renewalOptions = renewalOptions.Value;
    }

    [HttpPost("geocode")]
    public async Task<IActionResult> GeocodeAsync(CancellationToken cancellationToken)
    {
        var sites = await _internalApi.GetAsync<ICollection<SiteDto>>(null, $"api/task-runner/sites/geocode/pending?batchSize={_geocodingOptions.BatchSize}", cancellationToken);

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

        return Accepted(sites!.Count);
    }

    [HttpPost("process-renewal")]
    public async Task<IActionResult> ProcessRenewalAsync(CancellationToken cancellationToken)
    {
        var sites = await _internalApi.GetAsync<ICollection<SiteDto>>(
            null,
            $"api/task-runner/sites/renewal/pending?batchSize={_renewalOptions.BatchSize}",
            cancellationToken);

        await _queueService.EnsureQueueExistsAsync(QueueNames.BackflowTests.ProcessSiteRenewal, cancellationToken);

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = cancellationToken };

        await Parallel.ForEachAsync(sites!, parallelOptions, async (site, ct) =>
        {
            if (!ct.IsCancellationRequested)
            {
                await _queueService.SendMessageAsync(QueueNames.BackflowTests.ProcessSiteRenewal, site, ct);
            }
        });

        return Accepted(sites!.Count);
    }
}