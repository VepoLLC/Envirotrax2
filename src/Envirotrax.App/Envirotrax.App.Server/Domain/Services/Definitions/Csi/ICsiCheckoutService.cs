using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiCheckoutService
{
    Task<CsiCheckoutReceiptDto> CheckoutAsync(CsiCheckoutRequestDto request, CancellationToken cancellationToken);
}
