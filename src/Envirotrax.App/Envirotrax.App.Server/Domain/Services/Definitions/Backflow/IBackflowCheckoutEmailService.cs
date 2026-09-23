using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowCheckoutEmailService
{
    Task<List<BackflowCheckoutEmailResultDto>> SendTestReportsAsync(IEnumerable<BackflowTestDto> tests, string transactionId);
}
