namespace Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

public class ProfessionalReceiptDto
{
    public DateTime TransactionDate { get; set; }

    public string TransactionId { get; set; } = null!;

    public string TransactionType { get; set; } = null!;

    public List<string> AccountHolder { get; set; } = [];

    public string? ReferenceDescription { get; set; }

    public ReceiptReferenceDto? Reference { get; set; }

    public string? NameOnCard { get; set; }

    public string? CardNumber { get; set; }

    public decimal CardCharge { get; set; }

    public decimal BalanceAdjustment { get; set; }

    public decimal Total { get; set; }

    public bool ShowCreditCard { get; set; }

    public bool ShowBalanceAdjustment { get; set; }

    public bool ShowTotal { get; set; }
}

public class ReceiptReferenceDto
{
    public List<string> Headers { get; set; } = [];

    public List<ReceiptRowDto> Rows { get; set; } = [];
}

public class ReceiptRowDto
{
    public List<List<ReceiptLineDto>> Cells { get; set; } = [];
}

public class ReceiptLineDto
{
    public string? Label { get; set; }

    public string? Value { get; set; }
}
