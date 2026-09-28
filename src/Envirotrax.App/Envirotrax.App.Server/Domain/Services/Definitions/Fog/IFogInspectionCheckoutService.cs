using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogInspectionCheckoutService
{
    Task<FogInspectionCheckoutReceiptDto> CheckoutAsync(FogInspectionCheckoutRequestDto request, CancellationToken cancellationToken);
}
