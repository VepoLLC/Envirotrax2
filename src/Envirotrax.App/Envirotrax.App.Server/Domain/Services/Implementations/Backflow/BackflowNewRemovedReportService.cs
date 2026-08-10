using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowNewRemovedReportService(IBackflowNewRemovedReportRepository repository) : IBackflowNewRemovedReportService
{
    public Task<BackflowNewRemovedReportDto> GetNewRemovedAsync(CancellationToken cancellationToken)
    {
        return repository.GetNewRemovedAsync(cancellationToken);
    }
}
