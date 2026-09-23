using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowCheckoutRequestDto
{
    [Required]
    [StringLength(20)]
    [RegularExpression("^[A-Za-z0-9-]+$")]
    public string TransactionId { get; set; } = null!;

    [Required]
    [MinLength(1)]
    public List<BackflowCheckoutTestDto> Tests { get; set; } = [];

    public decimal ExpectedTotal { get; set; }

    public decimal ExpectedCcCharge { get; set; }

    public CreditCardPaymentDto? Card { get; set; }
}

public class BackflowCheckoutTestDto
{
    public int Id { get; set; }

    public bool EmailPdf { get; set; }
}
