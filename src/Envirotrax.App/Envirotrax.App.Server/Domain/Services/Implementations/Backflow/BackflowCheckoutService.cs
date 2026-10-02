using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.App.Server.Domain.Services.Implementations.Payments;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowCheckoutService
    : ProfessionalCheckoutService<BackflowTest, BackflowTestDto>, IBackflowCheckoutService
{
    private readonly IMapper _mapper;
    private readonly IBackflowTestRepository _testRepository;
    private readonly IBackflowTestNotificationService _notificationService;
    private readonly IBackflowCheckoutEmailService _checkoutEmailService;

    protected override ProfessionalTransactionType TransactionType => ProfessionalTransactionType.BackflowTestPayment;

    public BackflowCheckoutService(
        IMapper mapper,
        IAuthService authService,
        IBackflowTestRepository testRepository,
        IProfessionalRepository professionalRepository,
        IProfessionalTransactionRepository transactionRepository,
        IProfessionalPaymentService paymentService,
        IBackflowTestNotificationService notificationService,
        IBackflowCheckoutEmailService checkoutEmailService)
        : base(authService, professionalRepository, transactionRepository, paymentService)
    {
        _mapper = mapper;
        _testRepository = testRepository;
        _notificationService = notificationService;
        _checkoutEmailService = checkoutEmailService;
    }

    protected override Task<List<BackflowTest>> GetUnpaidItemsAsync(List<int> ids, CancellationToken cancellationToken)
    {
        return _testRepository.GetUnpaidForCheckoutAsync(ids, AuthService.ProfessionalId, GetBpatId(), cancellationToken);
    }

    protected override Task<int> MarkItemsPaidAsync(List<BackflowTest> tests, ProfessionalTransaction transaction, List<int> emailPdfIds)
    {
        var testIds = tests.Select(test => test.Id).ToList();

        return _testRepository.MarkPaidAsync(
            testIds, transaction.ProfessionalId, GetBpatId(), transaction.TransactionId!, transaction.TransactionDate, emailPdfIds, CancellationToken.None);
    }

    protected override Task<decimal> SumPaidAmountAsync(ProfessionalTransaction transaction)
    {
        return _testRepository.SumAmountByTransactionIdAsync(transaction.TransactionId!, transaction.ProfessionalId, CancellationToken.None);
    }

    protected override async Task OnItemsPaidAsync(List<BackflowTest> tests)
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

    protected override async Task<ProfessionalCheckoutReceiptDto<BackflowTestDto>> BuildReceiptAsync(ProfessionalTransaction transaction, CancellationToken cancellationToken)
    {
        var tests = await _testRepository.GetByTransactionIdAsync(transaction.TransactionId!, transaction.ProfessionalId, cancellationToken);
        var receipt = CreateReceipt(transaction);

        receipt.Items = _mapper.Map<List<BackflowTestDto>>(tests);

        return receipt;
    }

    protected override async Task OnPaymentCompletedAsync(ProfessionalCheckoutRequestDto request, ProfessionalCheckoutReceiptDto<BackflowTestDto> receipt)
    {
        await _notificationService.StartCheckingNotificationsAsync(request.Items.Select(item => item.Id), CancellationToken.None);

        var emailPdfTestIds = request.Items.Where(item => item.EmailPdf).Select(item => item.Id).ToHashSet();
        var testsToEmail = receipt.Items.Where(test => emailPdfTestIds.Contains(test.Id));

        receipt.EmailResults = await _checkoutEmailService.SendTestReportsAsync(testsToEmail, request.TransactionId);
    }

    private int? GetBpatId()
    {
        return AuthService.HasAnyRole(RoleDefinitions.Professionals.Admin) ? null : AuthService.UserId;
    }
}
