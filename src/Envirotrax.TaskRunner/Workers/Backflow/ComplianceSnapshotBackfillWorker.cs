
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;

namespace Envirotrax.TaskRunner.Workers.Backflow;

public class ComplianceSnapshotBackfillWorker : IQueueWorker<WaterSupplierDto>
{
    private readonly IInternalApiClientService _internalApi;

    public ComplianceSnapshotBackfillWorker(IInternalApiClientService internalApi)
    {
        _internalApi = internalApi;
    }

    public async Task DoWorkAsync(WaterSupplierDto? supplier, CancellationToken cancellationToken)
    {
        var apiRequest = new ServiceMessageDto<object>(waterSupplierId: supplier!.Id, loggedInUserId: null);

        await _internalApi.PostAsync<object, object>(
            "/api/task-runner/backflow-compliance-snapshots/backfill", apiRequest, cancellationToken);
    }
}
