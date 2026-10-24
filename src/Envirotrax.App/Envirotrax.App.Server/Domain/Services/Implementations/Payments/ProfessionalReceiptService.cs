using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Payments;

public class ProfessionalReceiptService : IProfessionalReceiptService
{
    private readonly IProfessionalTransactionRepository _transactionRepository;
    private readonly IEnumerable<IProfessionalReceiptReferenceProvider> _referenceProviders;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IPdfTemplateService _pdfTemplateService;

    public ProfessionalReceiptService(
        IProfessionalTransactionRepository transactionRepository,
        IEnumerable<IProfessionalReceiptReferenceProvider> referenceProviders,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        ITimeZoneHelperService timeZoneHelper,
        IPdfTemplateService pdfTemplateService)
    {
        _transactionRepository = transactionRepository;
        _referenceProviders = referenceProviders;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _timeZoneHelper = timeZoneHelper;
        _pdfTemplateService = pdfTemplateService;
    }

    public async Task<byte[]?> GeneratePdfAsync(string transactionId, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetMyByTransactionIdAsync(transactionId, cancellationToken);

        if (transaction == null)
        {
            return null;
        }

        var receipt = await BuildReceiptAsync(transaction, cancellationToken);

        return await _pdfTemplateService.GenerateAsync("Payments.ProfessionalReceipt", receipt);
    }

    private async Task<ProfessionalReceiptDto> BuildReceiptAsync(ProfessionalTransaction transaction, CancellationToken cancellationToken)
    {
        var receipt = new ProfessionalReceiptDto
        {
            TransactionDate = TimeZoneInfo.ConvertTimeFromUtc(transaction.TransactionDate, _timeZoneHelper.GetUserTimeZone()),
            TransactionId = transaction.TransactionId!,
            TransactionType = GetTransactionTypeDescription(transaction.TransactionType),
            AccountHolder = await BuildAccountHolderAsync(cancellationToken),
            NameOnCard = transaction.NameOnCard,
            CardNumber = transaction.CardNumber,
            CardCharge = transaction.CardCharge,
            BalanceAdjustment = transaction.BalanceAdjustment,
            Total = transaction.Amount,
            ShowCreditCard = transaction.CardCharge != 0,
            ShowBalanceAdjustment = transaction.BalanceAdjustment != 0,
            ShowTotal = transaction.TransactionType != ProfessionalTransactionType.BalanceAdjustment
                && (transaction.BalanceAdjustment != 0 || transaction.CardCharge == 0)
        };

        if (transaction.TransactionType == ProfessionalTransactionType.BalanceAdjustment)
        {
            receipt.ReferenceDescription = transaction.ReferenceDescription;
        }
        else
        {
            var provider = _referenceProviders.Single(provider => provider.TransactionType == transaction.TransactionType);

            receipt.Reference = await provider.GetReceiptReferenceAsync(transaction, cancellationToken);
        }

        return receipt;
    }

    private async Task<List<string>> BuildAccountHolderAsync(CancellationToken cancellationToken)
    {
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken)
            ?? throw new InvalidOperationException("Professional not found.");
        var user = await _professionalUserService.GetMyDataAsync(cancellationToken);

        return ReceiptFormatter.NonBlank(
            professional.Name,
            user?.ContactName,
            professional.Address,
            ReceiptFormatter.Join(", ", professional.City, ReceiptFormatter.Join(" ", professional.State?.Code, professional.ZipCode)));
    }

    private static string GetTransactionTypeDescription(ProfessionalTransactionType type)
    {
        return type switch
        {
            ProfessionalTransactionType.BackflowTestPayment => "Backflow Test Payment",
            ProfessionalTransactionType.CsiInspection => "CSI Submission Payment",
            ProfessionalTransactionType.BalanceAdjustment => "Balance Adjustment",
            ProfessionalTransactionType.FogInspectionPayment => "Inspection Payment",
            ProfessionalTransactionType.FogTripTicketPayment => "Trip Ticket Payment",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
