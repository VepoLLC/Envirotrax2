using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowNewRemovedReportService
{
    Task<BackflowNewRemovedReportDto> GetNewRemovedAsync(CancellationToken cancellationToken);
}
