using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.App.Server.Domain.Services.Implementations.Payments;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogInspectionCheckoutService
    : ProfessionalCheckoutService<FogInspection, FogInspectionCheckoutRequestDto, FogInspectionCheckoutReceiptDto>, IFogInspectionCheckoutService
{
    private readonly IMapper _mapper;
    private readonly IFogInspectionRepository _inspectionRepository;

    protected override ProfessionalTransactionType TransactionType => ProfessionalTransactionType.FogInspectionPayment;

    public FogInspectionCheckoutService(
        IMapper mapper,
        IAuthService authService,
        IFogInspectionRepository inspectionRepository,
        IProfessionalRepository professionalRepository,
        IProfessionalTransactionRepository transactionRepository,
        IProfessionalPaymentService paymentService)
        : base(authService, professionalRepository, transactionRepository, paymentService)
    {
        _mapper = mapper;
        _inspectionRepository = inspectionRepository;
    }

    protected override List<CheckoutItemDto> GetItems(FogInspectionCheckoutRequestDto request)
    {
        return request.Inspections;
    }

    protected override Task<List<FogInspection>> GetUnpaidItemsAsync(List<int> ids, CancellationToken cancellationToken)
    {
        return _inspectionRepository.GetUnpaidForCheckoutAsync(ids, AuthService.ProfessionalId, GetInspectorId(), cancellationToken);
    }

    protected override Task<int> MarkItemsPaidAsync(List<FogInspection> inspections, ProfessionalTransaction transaction, List<int> emailPdfIds)
    {
        var inspectionIds = inspections.Select(inspection => inspection.Id).ToList();

        return _inspectionRepository.MarkPaidAsync(
            inspectionIds, transaction.ProfessionalId, GetInspectorId(), transaction.TransactionId!, transaction.TransactionDate, CancellationToken.None);
    }

    protected override Task<decimal> SumPaidAmountAsync(ProfessionalTransaction transaction)
    {
        return _inspectionRepository.SumAmountByTransactionIdAsync(transaction.TransactionId!, transaction.ProfessionalId, CancellationToken.None);
    }

    protected override async Task<FogInspectionCheckoutReceiptDto> BuildReceiptAsync(ProfessionalTransaction transaction, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionRepository.GetByTransactionIdAsync(transaction.TransactionId!, transaction.ProfessionalId, cancellationToken);
        var receipt = CreateReceipt(transaction);

        receipt.Inspections = _mapper.Map<List<FogInspectionDto>>(inspections);

        return receipt;
    }

    private int? GetInspectorId()
    {
        return AuthService.HasAnyRole(RoleDefinitions.Professionals.Admin) ? null : AuthService.UserId;
    }
}
