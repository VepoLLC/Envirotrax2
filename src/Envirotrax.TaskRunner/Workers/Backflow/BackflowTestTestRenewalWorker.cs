
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;

namespace Envirotrax.TaskRunner.Workers.Backflow;

public class BackflowTestTestRenewalWorker : IQueueWorker<BackflowTestDto>
{
    private readonly IInternalApiClientService _internalApi;

    public BackflowTestTestRenewalWorker(IInternalApiClientService internalApi)
    {
        _internalApi = internalApi;
    }

    public async Task DoWorkAsync(BackflowTestDto? test, CancellationToken cancellationToken)
    {
        var apiRequest = new ServiceMessageDto<BackflowTestDto>(waterSupplierId: test!.WaterSupplier.Id, loggedInUserId: null)
        {
            Data = test
        };

        await _internalApi.PostAsync<BackflowTestDto, object>(
            $"/api/task-runner/backflow-tests/{test!.Id}/process-test-renewal", apiRequest, cancellationToken);
    }
}
