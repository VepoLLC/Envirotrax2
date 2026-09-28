namespace Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

public class ProfessionalCheckoutReceiptDto
{
    public string TransactionId { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceAdjustment { get; set; }

    public decimal CardCharge { get; set; }

    public string? NameOnCard { get; set; }

    public string? CardNumber { get; set; }

    public List<CheckoutEmailResultDto> EmailResults { get; set; } = [];
}
