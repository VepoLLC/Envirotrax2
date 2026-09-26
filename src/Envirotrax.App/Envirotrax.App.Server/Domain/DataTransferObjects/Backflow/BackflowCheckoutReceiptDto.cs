namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowCheckoutReceiptDto
{
    public string TransactionId { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceAdjustment { get; set; }

    public decimal CardCharge { get; set; }

    public string? NameOnCard { get; set; }

    public string? CardNumber { get; set; }

    public List<BackflowTestDto> Tests { get; set; } = [];

    public List<BackflowCheckoutEmailResultDto> EmailResults { get; set; } = [];
}
