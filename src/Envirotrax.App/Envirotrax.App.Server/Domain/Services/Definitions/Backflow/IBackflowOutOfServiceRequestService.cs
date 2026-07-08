using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowOutOfServiceRequestService : IService<BackflowOutOfServiceRequest, BackflowOutOfServiceRequestDto>
{
    Task<BackflowOutOfServiceRequestDto> SubmitAsync(BackflowOutOfServiceRequestDto dto, CancellationToken cancellationToken);

    Task<IEnumerable<BackflowTestDto>> GetReplacementCandidatesAsync(int testId, CancellationToken cancellationToken);
}
