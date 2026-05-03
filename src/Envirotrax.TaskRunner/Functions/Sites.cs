using System;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Configuration;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Envirotrax.TaskRunner.Functions;

public class Sites
{
    private readonly ILogger<Sites> _logger;
    private readonly IQueueService _queueService;
    private readonly IInternalApiClientService _internalApi;
    private readonly GeocodingOptions _geocodingOptions;

    public Sites(
        ILogger<Sites> logger,
        IQueueService queueService,
        IInternalApiClientService internalApi,
        IOptions<GeocodingOptions> geocodingOptions)
    {
        _logger = logger;
        _queueService = queueService;
        _internalApi = internalApi;
        _geocodingOptions = geocodingOptions.Value;
    }

    [Function("GeocodeSiteTimer")]
    public async Task GeocodeSiteTimerAsync([TimerTrigger("0 */15 * * * *")] TimerInfo _, CancellationToken cancellationToken)
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
    }

    [Function("GeocodeSite")]
    public async Task GeocodeSiteAsync([QueueTrigger(QueueNames.Sites.Geocode)] SiteDto site)
    {

    }
}