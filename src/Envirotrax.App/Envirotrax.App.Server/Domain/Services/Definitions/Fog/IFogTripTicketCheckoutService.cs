using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogTripTicketCheckoutService
{
    Task<ProfessionalCheckoutReceiptDto<FogTripTicketDto>> CheckoutAsync(ProfessionalCheckoutRequestDto request, CancellationToken cancellationToken);
}
