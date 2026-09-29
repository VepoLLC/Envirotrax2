using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogTripTicketCheckoutService
{
    Task<FogTripTicketCheckoutReceiptDto> CheckoutAsync(FogTripTicketCheckoutRequestDto request, CancellationToken cancellationToken);
}
