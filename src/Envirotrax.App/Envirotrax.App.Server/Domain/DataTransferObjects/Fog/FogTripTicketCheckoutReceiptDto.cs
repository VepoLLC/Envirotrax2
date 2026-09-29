using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogTripTicketCheckoutReceiptDto : ProfessionalCheckoutReceiptDto
{
    public List<FogTripTicketDto> Tickets { get; set; } = [];
}
