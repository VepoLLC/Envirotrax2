using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowCheckoutService
{
    Task<BackflowCheckoutReceiptDto> CheckoutAsync(BackflowCheckoutRequestDto request, CancellationToken cancellationToken);
}
