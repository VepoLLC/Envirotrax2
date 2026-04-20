using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiSubmissionService
{
    Task<CsiSubmissionCreateViewModel?> GetCreateViewModelAsync(int siteId, CancellationToken cancellationToken);
    Task<CsiInspectionDto> SubmitAsync(CsiSubmissionSaveRequest request, CancellationToken cancellationToken);
}
