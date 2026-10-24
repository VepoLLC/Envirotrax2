using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogTripTicketCheckoutService : IProfessionalReceiptReferenceProvider
{
    Task<ProfessionalCheckoutReceiptDto<FogTripTicketDto>> CheckoutAsync(ProfessionalCheckoutRequestDto request, CancellationToken cancellationToken);
}
