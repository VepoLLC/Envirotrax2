using System.Transactions;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Payments;

public class ProfessionalPaymentService : IProfessionalPaymentService
{
    private const string ContactSupportMessage = "Your payment was received but could not be recorded. Please contact support.";

    private readonly IAuthService _authService;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IProfessionalTransactionRepository _transactionRepository;
    private readonly IAuthorizeNetPaymentService _authorizeNetPaymentService;
    private readonly ILogger<ProfessionalPaymentService> _logger;

    public ProfessionalPaymentService(
        IAuthService authService,
        IProfessionalRepository professionalRepository,
        IProfessionalUserRepository professionalUserRepository,
        IProfessionalTransactionRepository transactionRepository,
        IAuthorizeNetPaymentService authorizeNetPaymentService,
        ILogger<ProfessionalPaymentService> logger)
    {
        _authService = authService;
        _professionalRepository = professionalRepository;
        _professionalUserRepository = professionalUserRepository;
        _transactionRepository = transactionRepository;
        _authorizeNetPaymentService = authorizeNetPaymentService;
        _logger = logger;
    }

    public async Task<IAsyncDisposable> AcquireBalanceLockAsync(CancellationToken cancellationToken)
    {
        return await _professionalRepository.TryAcquireBalanceLockAsync(_authService.ProfessionalId, cancellationToken)
            ?? throw new AppValidationException("Another payment for your company is in progress. Please try again in a minute.");
    }

    public async Task<ProfessionalTransaction?> GetProcessedTransactionAsync(string transactionId, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByTransactionIdAsync(transactionId, cancellationToken);

        if (transaction != null && transaction.ProfessionalId != _authService.ProfessionalId)
        {
            throw new AppValidationException("This payment could not be processed. Please try again.");
        }

        return transaction;
    }

    public async Task<AuthorizeNetChargeResult> ChargeCardAsync(CreditCardPaymentDto card, decimal amount, string transactionId)
    {
        AuthorizeNetChargeResult charge;

        try
        {
            charge = await _authorizeNetPaymentService.ChargeAsync(
                card.DataDescriptor,
                card.DataValue,
                amount,
                transactionId,
                new AuthorizeNetBillingInfo
                {
                    FirstName = card.BillingFirstName,
                    LastName = card.BillingLastName,
                    Address = card.BillingAddress,
                    City = card.BillingCity,
                    State = card.BillingState.Code,
                    Zip = card.BillingZipCode
                },
                CancellationToken.None);
        }
        catch (Exception chargeException)
        {
            _logger.LogCritical(chargeException,
                "Authorize.Net did not confirm payment {TransactionId} of {Amount} for professional {ProfessionalId}.",
                transactionId, amount, _authService.ProfessionalId);

            throw new AppValidationException("We could not confirm your payment. Please do not try again and contact support.");
        }

        if (!charge.IsApproved)
        {
            throw new AppValidationException($"Your card was declined: {charge.ErrorMessage}");
        }

        return charge;
    }

    public async Task RecordPaymentAsync(string transactionId, AuthorizeNetChargeResult? charge, decimal cardAmount, Func<Task> record)
    {
        try
        {
            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);

            await record();

            scope.Complete();
        }
        catch (Exception recordException) when (charge != null)
        {
            await HandleUnrecordedChargeAsync(transactionId, charge.TransactionId!, cardAmount, recordException);
        }
    }

    public async Task SaveBillingInfoAsync(CreditCardPaymentDto card, CancellationToken cancellationToken)
    {
        var professionalUser = await _professionalUserRepository.GetTrackedForUpdateAsync(_authService.UserId, cancellationToken)
            ?? throw new InvalidOperationException("Professional user not found.");

        professionalUser.BillingFirstName = card.BillingFirstName;
        professionalUser.BillingLastName = card.BillingLastName;
        professionalUser.BillingAddress = card.BillingAddress;
        professionalUser.BillingCity = card.BillingCity;
        professionalUser.BillingStateId = card.BillingState.Id;
        professionalUser.BillingZipCode = card.BillingZipCode;

        await _professionalUserRepository.SaveChangesAsync();
    }

    private async Task HandleUnrecordedChargeAsync(string transactionId, string gatewayTransactionId, decimal amount, Exception recordException)
    {
        try
        {
            var isRecorded = await _transactionRepository.GetByTransactionIdAsync(transactionId, CancellationToken.None) != null;

            if (isRecorded)
            {
                return;
            }
        }
        catch (Exception checkException)
        {
            _logger.LogCritical(new AggregateException(recordException, checkException),
                "Payment {TransactionId} (Authorize.Net {GatewayTransactionId}) of {Amount} for professional {ProfessionalId} was charged, but it is unknown whether it was recorded.",
                transactionId, gatewayTransactionId, amount, _authService.ProfessionalId);

            throw new AppValidationException(ContactSupportMessage);
        }

        if (await TryVoidAsync(gatewayTransactionId))
        {
            _logger.LogWarning(recordException,
                "Payment {TransactionId} (Authorize.Net {GatewayTransactionId}) was voided because it could not be recorded.",
                transactionId, gatewayTransactionId);

            var reason = recordException is AppValidationException validationException
                ? validationException.Message
                : "Your payment could not be completed.";

            throw new AppValidationException($"{reason} Your card payment was cancelled.");
        }

        _logger.LogCritical(recordException,
            "Payment {TransactionId} (Authorize.Net {GatewayTransactionId}) of {Amount} for professional {ProfessionalId} was charged but not recorded and could not be voided.",
            transactionId, gatewayTransactionId, amount, _authService.ProfessionalId);

        throw new AppValidationException(ContactSupportMessage);
    }

    private async Task<bool> TryVoidAsync(string gatewayTransactionId)
    {
        try
        {
            return await _authorizeNetPaymentService.VoidAsync(gatewayTransactionId, CancellationToken.None);
        }
        catch (Exception voidException)
        {
            _logger.LogError(voidException, "Voiding Authorize.Net transaction {GatewayTransactionId} failed.", gatewayTransactionId);
            return false;
        }
    }
}
