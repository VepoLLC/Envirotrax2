using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiCheckoutService : IProfessionalReceiptReferenceProvider
{
    Task<ProfessionalCheckoutReceiptDto<CsiInspectionDto>> CheckoutAsync(ProfessionalCheckoutRequestDto request, CancellationToken cancellationToken);
}
