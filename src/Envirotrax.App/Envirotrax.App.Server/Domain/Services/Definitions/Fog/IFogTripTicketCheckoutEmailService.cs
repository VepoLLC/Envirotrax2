using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogTripTicketCheckoutEmailService
{
    Task<List<CheckoutEmailResultDto>> SendTripTicketReportsAsync(IEnumerable<FogTripTicketDto> tickets, string transactionId);
}
