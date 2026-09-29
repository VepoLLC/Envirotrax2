using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogTripTicketCheckoutRequestDto : ProfessionalCheckoutRequestDto
{
    [Required]
    [MinLength(1)]
    public List<CheckoutItemDto> Tickets { get; set; } = [];
}
