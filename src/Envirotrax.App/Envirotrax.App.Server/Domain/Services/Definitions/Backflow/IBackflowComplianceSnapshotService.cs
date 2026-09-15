namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowComplianceSnapshotService
{
    Task GenerateSnapshotAsync(DateTime reportDate, CancellationToken cancellationToken);

    Task BackfillAsync(CancellationToken cancellationToken);
}
