using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowCheckoutRequestDto : ProfessionalCheckoutRequestDto
{
    [Required]
    [MinLength(1)]
    public List<CheckoutItemDto> Tests { get; set; } = [];
}
