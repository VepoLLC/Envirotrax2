using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

public class ProfessionalCheckoutRequestDto
{
    [Required]
    [StringLength(20)]
    [RegularExpression("^[A-Za-z0-9-]+$")]
    public string TransactionId { get; set; } = null!;

    public decimal ExpectedTotal { get; set; }

    public decimal ExpectedCardCharge { get; set; }

    public CreditCardPaymentDto? Card { get; set; }
}
