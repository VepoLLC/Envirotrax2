using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogInspectionCheckoutService : IProfessionalReceiptReferenceProvider
{
    Task<ProfessionalCheckoutReceiptDto<FogInspectionDto>> CheckoutAsync(ProfessionalCheckoutRequestDto request, CancellationToken cancellationToken);
}
