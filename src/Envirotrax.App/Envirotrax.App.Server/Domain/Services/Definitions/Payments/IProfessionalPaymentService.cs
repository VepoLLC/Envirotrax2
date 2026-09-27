using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Payments;

public interface IProfessionalPaymentService
{
    Task<AuthorizeNetChargeResult> ChargeCardAsync(CreditCardPaymentDto card, decimal amount, string transactionId);

    Task RecordPaymentAsync(string transactionId, AuthorizeNetChargeResult? charge, decimal cardAmount, Func<Task> record);

    Task SaveBillingInfoAsync(CreditCardPaymentDto card, CancellationToken cancellationToken);
}
