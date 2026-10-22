using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

public class ProfessionalTransactionDto
{
    public int Id { get; set; }

    public DateTime TransactionDate { get; set; }

    public string TransactionId { get; set; } = null!;

    public ReferencedProfessionalUserDto? User { get; set; }

    public ProfessionalTransactionType TransactionType { get; set; }

    public string? ReferenceDescription { get; set; }

    public decimal BalanceAdjustment { get; set; }

    public decimal CardCharge { get; set; }

    public decimal Amount { get; set; }
}
