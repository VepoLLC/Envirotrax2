using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowCheckoutService
{
    Task<ProfessionalCheckoutReceiptDto<BackflowTestDto>> CheckoutAsync(ProfessionalCheckoutRequestDto request, CancellationToken cancellationToken);
}
