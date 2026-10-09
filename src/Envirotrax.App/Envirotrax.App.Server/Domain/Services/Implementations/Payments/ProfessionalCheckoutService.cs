using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Payments;

public abstract class ProfessionalCheckoutService<TItem, TItemDto>
    where TItem : IPayableModel
    where TItemDto : IRedactableMailingInfoDto
{
    private const string AmountsChangedMessage = "The amounts have changed. Please refresh the page and try again.";

    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalTransactionRepository _transactionRepository;
    private readonly IProfessionalPaymentService _paymentService;
    private readonly IMailingInfoRedactionService _mailingInfoRedactionService;

    protected IAuthService AuthService { get; private set; }

    protected abstract ProfessionalTransactionType TransactionType { get; }

    public ProfessionalCheckoutService(
        IAuthService authService,
        IProfessionalRepository professionalRepository,
        IProfessionalTransactionRepository transactionRepository,
        IProfessionalPaymentService paymentService,
        IMailingInfoRedactionService mailingInfoRedactionService)
    {
        AuthService = authService;
        _professionalRepository = professionalRepository;
        _transactionRepository = transactionRepository;
        _paymentService = paymentService;
        _mailingInfoRedactionService = mailingInfoRedactionService;
    }

    public async Task<ProfessionalCheckoutReceiptDto<TItemDto>> CheckoutAsync(ProfessionalCheckoutRequestDto request, CancellationToken cancellationToken)
    {
        var itemIds = request.Items.Select(item => item.Id).ToList();

        if (itemIds.Distinct().Count() != itemIds.Count)
        {
            throw new AppValidationException("Each item can only be paid once.");
        }

        var (transaction, isNewPayment) = await PayAsync(request, itemIds, cancellationToken);
        var receipt = await BuildReceiptAsync(transaction, CancellationToken.None);

        // Redact before OnPaymentCompletedAsync: the report PDFs it emails are built from these items and
        // go to the paying professional, just like the receipt itself.
        receipt.Items = await _mailingInfoRedactionService.RedactAsync(receipt.Items, CancellationToken.None);

        if (isNewPayment)
        {
            await OnPaymentCompletedAsync(request, receipt);
        }

        return receipt;
    }

    protected abstract Task<List<TItem>> GetUnpaidItemsAsync(List<int> ids, CancellationToken cancellationToken);

    protected abstract Task<int> MarkItemsPaidAsync(List<TItem> items, ProfessionalTransaction transaction, List<int> emailPdfIds);

    protected abstract Task<decimal> SumPaidAmountAsync(ProfessionalTransaction transaction);

    protected abstract Task<ProfessionalCheckoutReceiptDto<TItemDto>> BuildReceiptAsync(ProfessionalTransaction transaction, CancellationToken cancellationToken);

    protected virtual Task OnItemsPaidAsync(List<TItem> items)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnPaymentCompletedAsync(ProfessionalCheckoutRequestDto request, ProfessionalCheckoutReceiptDto<TItemDto> receipt)
    {
        return Task.CompletedTask;
    }

    protected ProfessionalCheckoutReceiptDto<TItemDto> CreateReceipt(ProfessionalTransaction transaction)
    {
        return new ProfessionalCheckoutReceiptDto<TItemDto>
        {
            TransactionId = transaction.TransactionId!,
            TransactionDate = transaction.TransactionDate,
            Amount = transaction.Amount,
            BalanceAdjustment = transaction.BalanceAdjustment,
            CardCharge = transaction.CardCharge,
            NameOnCard = transaction.NameOnCard,
            CardNumber = transaction.CardNumber
        };
    }

    private async Task<(ProfessionalTransaction Transaction, bool IsNewPayment)> PayAsync(
        ProfessionalCheckoutRequestDto request,
        List<int> itemIds,
        CancellationToken cancellationToken)
    {
        var processedTransaction = await _transactionRepository.GetByTransactionIdAsync(request.TransactionId, cancellationToken);

        if (processedTransaction != null)
        {
            if (processedTransaction.TransactionType != TransactionType)
            {
                throw new AppValidationException("Please try again.");
            }

            return (processedTransaction, false);
        }

        var items = await GetUnpaidItemsAsync(itemIds, cancellationToken);

        if (items.Count != itemIds.Count)
        {
            throw new AppValidationException(AmountsChangedMessage);
        }

        var amounts = await CalculateAmountsAsync(items, cancellationToken);

        if (amounts.Total != RoundToCents(request.ExpectedTotal) || amounts.CardCharge != RoundToCents(request.ExpectedCardCharge))
        {
            throw new AppValidationException(AmountsChangedMessage);
        }

        var charge = amounts.CardCharge > 0 ? await ChargeCardAsync(request, amounts.CardCharge) : null;
        var transaction = BuildTransaction(request, amounts, charge);

        await _paymentService.RecordPaymentAsync(
            request.TransactionId,
            charge,
            amounts.CardCharge,
            () => RecordCheckoutAsync(request, items, amounts, transaction));

        return (transaction, true);
    }

    private async Task<CheckoutAmounts> CalculateAmountsAsync(List<TItem> items, CancellationToken cancellationToken)
    {
        var professional = await _professionalRepository.GetNoIncludesAsync(AuthService.ProfessionalId, cancellationToken)
            ?? throw new InvalidOperationException("Professional not found.");

        var total = RoundToCents(items.Sum(item => item.Amount));
        var totalShare = RoundToCents(items.Sum(item => item.AmountShare));
        var availableBalance = Math.Round(professional.AccountBalance, 2, MidpointRounding.ToZero);
        var fromBalance = Math.Min(availableBalance, total);

        return new CheckoutAmounts(total, totalShare, fromBalance, total - fromBalance);
    }

    private async Task<AuthorizeNetChargeResult> ChargeCardAsync(ProfessionalCheckoutRequestDto request, decimal amount)
    {
        if (request.Card == null)
        {
            throw new AppValidationException("Credit card information is required.");
        }

        return await _paymentService.ChargeCardAsync(request.Card, amount, request.TransactionId);
    }

    private ProfessionalTransaction BuildTransaction(ProfessionalCheckoutRequestDto request, CheckoutAmounts amounts, AuthorizeNetChargeResult? charge)
    {
        return new ProfessionalTransaction
        {
            TransactionDate = DateTime.UtcNow,
            ProfessionalId = AuthService.ProfessionalId,
            UserId = AuthService.UserId,
            TransactionId = request.TransactionId,
            TransactionType = TransactionType,
            BalanceAdjustment = -amounts.FromBalance,
            CardCharge = amounts.CardCharge,
            Amount = amounts.Total,
            AmountShare = amounts.TotalShare,
            NameOnCard = charge != null ? $"{request.Card!.BillingFirstName} {request.Card.BillingLastName}" : null,
            CardNumber = charge?.CardNumber
        };
    }

    private async Task RecordCheckoutAsync(ProfessionalCheckoutRequestDto request, List<TItem> items, CheckoutAmounts amounts, ProfessionalTransaction transaction)
    {
        if (amounts.FromBalance > 0 && !await _professionalRepository.TryDebitBalanceAsync(transaction.ProfessionalId, amounts.FromBalance, CancellationToken.None))
        {
            throw new AppValidationException(AmountsChangedMessage);
        }

        var emailPdfIds = request.Items.Where(item => item.EmailPdf).Select(item => item.Id).ToList();

        var paidCount = await MarkItemsPaidAsync(items, transaction, emailPdfIds);
        var paidTotal = RoundToCents(await SumPaidAmountAsync(transaction));

        if (paidCount != items.Count || paidTotal != amounts.Total)
        {
            throw new AppValidationException(AmountsChangedMessage);
        }

        await OnItemsPaidAsync(items);

        await _transactionRepository.AddAsync(transaction);

        if (request.Card != null)
        {
            await _paymentService.SaveBillingInfoAsync(request.Card, CancellationToken.None);
        }
    }

    private static decimal RoundToCents(decimal amount)
    {
        return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    private record CheckoutAmounts(decimal Total, decimal TotalShare, decimal FromBalance, decimal CardCharge);
}
