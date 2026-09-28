using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiCheckoutRequestDto : ProfessionalCheckoutRequestDto
{
    [Required]
    [MinLength(1)]
    public List<CheckoutItemDto> Inspections { get; set; } = [];
}
