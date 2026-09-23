namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowCheckoutReceiptDto
{
    public string TransactionId { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceAdjustment { get; set; }

    public decimal CcCharge { get; set; }

    public string? CCNameOnCard { get; set; }

    public string? CCNumber { get; set; }

    public List<BackflowTestDto> Tests { get; set; } = [];

    public List<BackflowCheckoutEmailResultDto> EmailResults { get; set; } = [];
}
