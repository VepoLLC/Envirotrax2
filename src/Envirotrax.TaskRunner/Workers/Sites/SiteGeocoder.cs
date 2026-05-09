
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
        var apiRequest = new ServiceMessageDto<SiteDto>(waterSupplierId: site!.WaterSupplier.Id, loggedInUserId: null)
        {
            Data = site
        };

        await _internalApi.PostAsync<SiteDto, SiteDto>($"/api/task-runner/sites/{site!.Id}/geocode?assignGisArea={_geocodingOptions.AssignGisAreas}", apiRequest);
    }
}