using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowNewRemovedReportRepository
{
    Task<BackflowNewRemovedReportDto> GetNewRemovedAsync(CancellationToken cancellationToken);
}
