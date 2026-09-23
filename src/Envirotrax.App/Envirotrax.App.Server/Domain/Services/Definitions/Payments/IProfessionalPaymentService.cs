using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Payments;

public interface IProfessionalPaymentService
{
    Task<IAsyncDisposable> AcquireBalanceLockAsync(CancellationToken cancellationToken);

    Task<ProfessionalTransaction?> GetProcessedTransactionAsync(string transactionId, CancellationToken cancellationToken);

    Task<AuthorizeNetChargeResult> ChargeCardAsync(CreditCardPaymentDto card, decimal amount, string transactionId);

    Task RecordPaymentAsync(string transactionId, AuthorizeNetChargeResult? charge, decimal cardAmount, Func<Task> record);

    Task SaveBillingInfoAsync(CreditCardPaymentDto card, CancellationToken cancellationToken);
}
