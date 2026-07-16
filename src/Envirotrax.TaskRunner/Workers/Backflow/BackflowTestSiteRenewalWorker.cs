
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;

namespace Envirotrax.TaskRunner.Workers.Backflow;

public class BackflowTestSiteRenewalWorker : IQueueWorker<SiteDto>
{
    private readonly IInternalApiClientService _internalApi;

    public BackflowTestSiteRenewalWorker(IInternalApiClientService internalApi)
    {
        _internalApi = internalApi;
    }

    public async Task DoWorkAsync(SiteDto? site, CancellationToken cancellationToken)
    {
        var apiRequest = new ServiceMessageDto<SiteDto>(waterSupplierId: site!.WaterSupplier.Id, loggedInUserId: null)
        {
            Data = site
        };

        await _internalApi.PostAsync<SiteDto, object>($"/api/task-runner/backflow-tests/sites/{site!.Id}/process-renewal", apiRequest, cancellationToken);
    }
}
