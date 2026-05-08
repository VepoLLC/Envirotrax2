
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Configuration;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;
using Microsoft.Extensions.Options;

namespace Envirotrax.TaskRunner.Workers.Sites;

public class SiteGeocoder : IQueueWorker<SiteDto>
{
    private readonly IInternalApiClientService _internalApi;
    private readonly GeocodingOptions _geocodingOptions;

    public SiteGeocoder(IInternalApiClientService internalApi, IOptions<GeocodingOptions> options)
    {
        _internalApi = internalApi;
        _geocodingOptions = options.Value;
    }

    public async Task DoWorkAsync(SiteDto? site, CancellationToken cancellationToken)
    {
        await _internalApi.PostAsync<SiteDto, SiteDto>($"/api/task-runner/sites?assignGisArea={_geocodingOptions.AssignGisAreas}", new ServiceMessageDto<SiteDto>(site!.WaterSupplier.Id)
        {
            Data = site
        });
    }
}