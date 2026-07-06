
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;

namespace Envirotrax.TaskRunner.Workers.Backflow;

public class BackflowTestRenewalWorker : IQueueWorker<BackflowTestDto>
{
    private readonly IInternalApiClientService _internalApi;

    public BackflowTestRenewalWorker(IInternalApiClientService internalApi)
    {
        _internalApi = internalApi;
    }

    public async Task DoWorkAsync(BackflowTestDto? test, CancellationToken cancellationToken)
    {
        var apiRequest = new ServiceMessageDto<BackflowTestDto>(waterSupplierId: test!.WaterSupplier.Id, loggedInUserId: null)
        {
            Data = test
        };

        await _internalApi.PostAsync<BackflowTestDto, BackflowTestDto>($"/api/task-runner/backflow-tests/{test!.Id}/extend-date", apiRequest, cancellationToken);
    }
}
