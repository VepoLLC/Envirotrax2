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
    private readonly IInternalApiClientService _internalApi;
    private readonly GeocodingOptions _geocodingOptions;

    public Sites(
        ILogger<Sites> logger,
        IInternalApiClientService internalApi,
        IOptions<GeocodingOptions> geocodingOptions)
    {
        _logger = logger;
        _internalApi = internalApi;
        _geocodingOptions = geocodingOptions.Value;
    }

    [Function("GeocodeSiteTimer")]
    public async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo _)
    {
        var sites = await _internalApi.GetAsync<ICollection<SiteDto>>(null, $"api/task-runner/sites/geocode/pending?batchSize={_geocodingOptions.BatchSize}");

        _logger.LogInformation("Receiving {count} sites pending geocoding.", sites!.Count);


    }
}