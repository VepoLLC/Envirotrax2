using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiCheckoutEmailService
{
    Task<List<CheckoutEmailResultDto>> SendInspectionReportsAsync(IEnumerable<CsiInspectionDto> inspections, string transactionId);
}
