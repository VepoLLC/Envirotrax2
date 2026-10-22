using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowCheckoutService : IProfessionalReceiptReferenceProvider
{
    Task<ProfessionalCheckoutReceiptDto<BackflowTestDto>> CheckoutAsync(ProfessionalCheckoutRequestDto request, CancellationToken cancellationToken);
}
