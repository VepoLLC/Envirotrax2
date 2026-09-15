
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;

namespace Envirotrax.TaskRunner.Workers.Backflow;

public class ComplianceSnapshotWorker : IQueueWorker<ComplianceSnapshotMessageDto>
{
    private readonly IInternalApiClientService _internalApi;

    public ComplianceSnapshotWorker(IInternalApiClientService internalApi)
    {
        _internalApi = internalApi;
    }

    public async Task DoWorkAsync(ComplianceSnapshotMessageDto? message, CancellationToken cancellationToken)
    {
        var apiRequest = new ServiceMessageDto<GenerateComplianceSnapshotRequest>(waterSupplierId: message!.WaterSupplier.Id, loggedInUserId: null)
        {
            Data = new GenerateComplianceSnapshotRequest { ReportDate = message.ReportDate }
        };

        await _internalApi.PostAsync<GenerateComplianceSnapshotRequest, object>(
            "/api/task-runner/backflow-compliance-snapshots", apiRequest, cancellationToken);
    }
}
