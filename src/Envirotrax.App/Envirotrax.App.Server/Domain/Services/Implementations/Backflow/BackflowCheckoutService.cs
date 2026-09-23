using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.Common;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowCheckoutService : IBackflowCheckoutService
{
    private const string AmountsChangedMessage = "The amounts have changed. Please refresh the page and try again.";

    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly IBackflowTestRepository _testRepository;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalTransactionRepository _transactionRepository;
    private readonly IProfessionalPaymentService _paymentService;
    private readonly IBackflowTestNotificationService _notificationService;
    private readonly IBackflowCheckoutEmailService _checkoutEmailService;

    public BackflowCheckoutService(
        IMapper mapper,
        IAuthService authService,
        IBackflowTestRepository testRepository,
        IProfessionalRepository professionalRepository,
        IProfessionalTransactionRepository transactionRepository,
        IProfessionalPaymentService paymentService,
        IBackflowTestNotificationService notificationService,
        IBackflowCheckoutEmailService checkoutEmailService)
    {
        _mapper = mapper;
        _authService = authService;
        _testRepository = testRepository;
        _professionalRepository = professionalRepository;
        _transactionRepository = transactionRepository;
        _paymentService = paymentService;
        _notificationService = notificationService;
        _checkoutEmailService = checkoutEmailService;
    }

    public async Task<BackflowCheckoutReceiptDto> CheckoutAsync(BackflowCheckoutRequestDto request, CancellationToken cancellationToken)
    {
        var testIds = request.Tests.Select(test => test.Id).ToList();

        if (testIds.Distinct().Count() != testIds.Count)
        {
            throw new AppValidationException("Each test can only be paid once.");
        }

        var (transaction, isNewPayment) = await PayUnderBalanceLockAsync(request, testIds, cancellationToken);
        var receipt = await BuildReceiptAsync(transaction, CancellationToken.None);

        if (isNewPayment)
        {
            await _notificationService.StartCheckingNotificationsAsync(testIds, CancellationToken.None);

            var emailPdfTestIds = request.Tests.Where(test => test.EmailPdf).Select(test => test.Id).ToHashSet();
            var testsToEmail = receipt.Tests.Where(test => emailPdfTestIds.Contains(test.Id));

            receipt.EmailResults = await _checkoutEmailService.SendTestReportsAsync(testsToEmail, transaction.TransactionId);
        }

        return receipt;
    }

    private async Task<(ProfessionalTransaction Transaction, bool IsNewPayment)> PayUnderBalanceLockAsync(
        BackflowCheckoutRequestDto request,
        List<int> testIds,
        CancellationToken cancellationToken)
    {
        await using var balanceLock = await _paymentService.AcquireBalanceLockAsync(cancellationToken);

        var processedTransaction = await _paymentService.GetProcessedTransactionAsync(request.TransactionId, cancellationToken);

        if (processedTransaction != null)
        {
            return (processedTransaction, false);
        }

        var bpatId = _authService.HasAnyRole(RoleDefinitions.Professionals.Admin) ? (int?)null : _authService.UserId;
        var tests = await _testRepository.GetUnpaidForCheckoutAsync(testIds, _authService.ProfessionalId, bpatId, cancellationToken);

        if (tests.Count != testIds.Count)
        {
            throw new AppValidationException(AmountsChangedMessage);
        }

        var amounts = await CalculateAmountsAsync(tests, cancellationToken);

        if (amounts.Total != RoundToCents(request.ExpectedTotal) || amounts.CcCharge != RoundToCents(request.ExpectedCcCharge))
        {
            throw new AppValidationException(AmountsChangedMessage);
        }

        var charge = amounts.CcCharge > 0 ? await ChargeCardAsync(request, amounts.CcCharge) : null;
        var transaction = BuildTransaction(request, amounts, charge);

        await _paymentService.RecordPaymentAsync(
            request.TransactionId,
            charge,
            amounts.CcCharge,
            () => RecordCheckoutAsync(request, tests, bpatId, amounts, transaction));

        return (transaction, true);
    }

    private async Task<CheckoutAmounts> CalculateAmountsAsync(List<BackflowTest> tests, CancellationToken cancellationToken)
    {
        var professional = await _professionalRepository.GetNoIncludesAsync(_authService.ProfessionalId, cancellationToken)
            ?? throw new InvalidOperationException("Professional not found.");

        var total = RoundToCents(tests.Sum(test => test.Amount));
        var totalShare = RoundToCents(tests.Sum(test => test.AmountShare));
        var availableBalance = Math.Round(professional.AccountBalance, 2, MidpointRounding.ToZero);
        var fromBalance = Math.Min(availableBalance, total);

        return new CheckoutAmounts(total, totalShare, fromBalance, total - fromBalance);
    }

    private async Task<AuthorizeNetChargeResult> ChargeCardAsync(BackflowCheckoutRequestDto request, decimal amount)
    {
        if (request.Card == null)
        {
            throw new AppValidationException("Credit card information is required.");
        }

        return await _paymentService.ChargeCardAsync(request.Card, amount, request.TransactionId);
    }

    private ProfessionalTransaction BuildTransaction(BackflowCheckoutRequestDto request, CheckoutAmounts amounts, AuthorizeNetChargeResult? charge)
    {
        return new ProfessionalTransaction
        {
            TransactionDate = DateTime.UtcNow,
            ProfessionalId = _authService.ProfessionalId,
            UserId = _authService.UserId,
            TransactionId = request.TransactionId,
            TransactionType = ProfessionalTransactionType.BackflowTestPayment,
            BalanceAdjustment = -amounts.FromBalance,
            CcCharge = amounts.CcCharge,
            Amount = amounts.Total,
            AmountShare = amounts.TotalShare,
            CCNameOnCard = charge != null ? $"{request.Card!.BillingFirstName} {request.Card.BillingLastName}" : null,
            CCNumber = charge?.CardNumber
        };
    }

    private async Task RecordCheckoutAsync(
        BackflowCheckoutRequestDto request,
        List<BackflowTest> tests,
        int? bpatId,
        CheckoutAmounts amounts,
        ProfessionalTransaction transaction)
    {
        var professionalId = _authService.ProfessionalId;

        if (amounts.FromBalance > 0 && !await _professionalRepository.TryDebitBalanceAsync(professionalId, amounts.FromBalance, CancellationToken.None))
        {
            throw new AppValidationException(AmountsChangedMessage);
        }

        var testIds = tests.Select(test => test.Id).ToList();
        var emailPdfTestIds = request.Tests.Where(test => test.EmailPdf).Select(test => test.Id).ToList();

        var paidCount = await _testRepository.MarkPaidAsync(
            testIds, professionalId, bpatId, transaction.TransactionId, transaction.TransactionDate, emailPdfTestIds, CancellationToken.None);

        var paidTotal = RoundToCents(await _testRepository.SumAmountByTransactionIdAsync(transaction.TransactionId, CancellationToken.None));

        if (paidCount != tests.Count || paidTotal != amounts.Total)
        {
            throw new AppValidationException(AmountsChangedMessage);
        }

        await UpdateIsCurrentAsync(tests);

        await _transactionRepository.AddAsync(transaction);

        if (request.Card != null)
        {
            await _paymentService.SaveBillingInfoAsync(request.Card, CancellationToken.None);
        }
    }

    private async Task UpdateIsCurrentAsync(List<BackflowTest> tests)
    {
        foreach (var test in tests.OrderBy(test => test.CreatedTime))
        {
            var previousTest = test.UnknownSerialNumber
                ? null
                : await _testRepository.FindPreviousCurrentTestAsync(test, CancellationToken.None);

            var replacesPreviousTest = previousTest == null
                || (test.TestDate ?? DateTime.MinValue) >= (previousTest.TestDate ?? DateTime.MinValue);

            if (previousTest != null && replacesPreviousTest)
            {
                await _testRepository.SetIsCurrentAsync(previousTest.Id, false, CancellationToken.None);
            }

            await _testRepository.SetIsCurrentAsync(test.Id, replacesPreviousTest, CancellationToken.None);
        }
    }

    private async Task<BackflowCheckoutReceiptDto> BuildReceiptAsync(ProfessionalTransaction transaction, CancellationToken cancellationToken)
    {
        var tests = await _testRepository.GetByTransactionIdAsync(transaction.TransactionId, transaction.ProfessionalId, cancellationToken);

        return new BackflowCheckoutReceiptDto
        {
            TransactionId = transaction.TransactionId,
            TransactionDate = transaction.TransactionDate,
            Amount = transaction.Amount,
            BalanceAdjustment = transaction.BalanceAdjustment,
            CcCharge = transaction.CcCharge,
            CCNameOnCard = transaction.CCNameOnCard,
            CCNumber = transaction.CCNumber,
            Tests = _mapper.Map<List<BackflowTestDto>>(tests)
        };
    }

    private static decimal RoundToCents(decimal amount)
    {
        return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    private record CheckoutAmounts(decimal Total, decimal TotalShare, decimal FromBalance, decimal CcCharge);
}
