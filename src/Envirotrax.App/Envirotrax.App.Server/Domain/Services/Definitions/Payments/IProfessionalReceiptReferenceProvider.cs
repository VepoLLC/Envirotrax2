using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Payments;

public interface IProfessionalReceiptReferenceProvider
{
    ProfessionalTransactionType TransactionType { get; }

    Task<ReceiptReferenceDto> GetReceiptReferenceAsync(ProfessionalTransaction transaction, CancellationToken cancellationToken);
}
