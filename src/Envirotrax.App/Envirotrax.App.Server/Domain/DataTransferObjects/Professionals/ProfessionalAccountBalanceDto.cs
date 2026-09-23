using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalAccountBalanceDto : CreditCardPaymentDto
{
    [Required]
    [StringLength(20)]
    [RegularExpression("^[A-Za-z0-9-]+$")]
    public string TransactionId { get; set; } = null!;

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal AmountToAdd { get; set; }
}
