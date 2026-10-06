using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.App.Server.Domain.Services.Implementations.Payments;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiCheckoutService
    : ProfessionalCheckoutService<CsiInspection, CsiInspectionDto>, ICsiCheckoutService
{
    private readonly IMapper _mapper;
    private readonly ICsiInspectionRepository _inspectionRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly ICsiCheckoutEmailService _checkoutEmailService;
    private readonly ICsiInspectionAssemblyService _assemblyService;

    protected override ProfessionalTransactionType TransactionType => ProfessionalTransactionType.CsiInspection;

    public CsiCheckoutService(
        IMapper mapper,
        IAuthService authService,
        ICsiInspectionRepository inspectionRepository,
        ISiteRepository siteRepository,
        IProfessionalRepository professionalRepository,
        IProfessionalTransactionRepository transactionRepository,
        IProfessionalPaymentService paymentService,
        ICsiCheckoutEmailService checkoutEmailService,
        ICsiInspectionAssemblyService assemblyService)
        : base(authService, professionalRepository, transactionRepository, paymentService)
    {
        _mapper = mapper;
        _inspectionRepository = inspectionRepository;
        _siteRepository = siteRepository;
        _checkoutEmailService = checkoutEmailService;
        _assemblyService = assemblyService;
    }

    protected override Task<List<CsiInspection>> GetUnpaidItemsAsync(List<int> ids, CancellationToken cancellationToken)
    {
        return _inspectionRepository.GetUnpaidForCheckoutAsync(ids, AuthService.ProfessionalId, GetInspectorId(), cancellationToken);
    }

    protected override async Task<int> MarkItemsPaidAsync(List<CsiInspection> inspections, ProfessionalTransaction transaction, List<int> emailPdfIds)
    {
        var inspectionIds = inspections.Select(inspection => inspection.Id).ToList();

        var paidCount = await _inspectionRepository.MarkPaidAsync(
            inspectionIds, transaction.ProfessionalId, GetInspectorId(), transaction.TransactionId!, transaction.TransactionDate, emailPdfIds, CancellationToken.None);

        await _assemblyService.MarkPaidAsync(inspections, transaction.TransactionId!, transaction.TransactionDate, CancellationToken.None);

        return paidCount;
    }

    protected override Task<decimal> SumPaidAmountAsync(ProfessionalTransaction transaction)
    {
        return _inspectionRepository.SumAmountByTransactionIdAsync(transaction.TransactionId!, transaction.ProfessionalId, CancellationToken.None);
    }

    protected override Task OnItemsPaidAsync(List<CsiInspection> inspections)
    {
        var siteIds = inspections.Select(inspection => inspection.SiteId).Distinct().ToList();

        return _siteRepository.ClearNeedsCsiInspectionAsync(siteIds);
    }

    protected override async Task<ProfessionalCheckoutReceiptDto<CsiInspectionDto>> BuildReceiptAsync(ProfessionalTransaction transaction, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionRepository.GetByTransactionIdAsync(transaction.TransactionId!, transaction.ProfessionalId, cancellationToken);
        var receipt = CreateReceipt(transaction);

        receipt.Items = _mapper.Map<List<CsiInspectionDto>>(inspections);

        return receipt;
    }

    protected override async Task OnPaymentCompletedAsync(ProfessionalCheckoutRequestDto request, ProfessionalCheckoutReceiptDto<CsiInspectionDto> receipt)
    {
        var emailPdfInspectionIds = request.Items.Where(item => item.EmailPdf).Select(item => item.Id).ToHashSet();
        var inspectionsToEmail = receipt.Items.Where(inspection => emailPdfInspectionIds.Contains(inspection.Id));

        receipt.EmailResults = await _checkoutEmailService.SendInspectionReportsAsync(inspectionsToEmail, request.TransactionId);
    }

    private int? GetInspectorId()
    {
        return AuthService.HasAnyRole(RoleDefinitions.Professionals.Admin) ? null : AuthService.UserId;
    }
}
