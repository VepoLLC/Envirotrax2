using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowCheckoutEmailService
{
    Task<List<CheckoutEmailResultDto>> SendTestReportsAsync(IEnumerable<BackflowTestDto> tests, string transactionId);
}
