using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.App.Server.Domain.Services.Implementations.Payments;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogTripTicketCheckoutService
    : ProfessionalCheckoutService<FogTripTicket, FogTripTicketCheckoutRequestDto, FogTripTicketCheckoutReceiptDto>, IFogTripTicketCheckoutService
{
    private readonly IMapper _mapper;
    private readonly IFogTripTicketRepository _ticketRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IFogTripTicketCheckoutEmailService _checkoutEmailService;

    protected override ProfessionalTransactionType TransactionType => ProfessionalTransactionType.FogTripTicketPayment;

    public FogTripTicketCheckoutService(
        IMapper mapper,
        IAuthService authService,
        IFogTripTicketRepository ticketRepository,
        ISiteRepository siteRepository,
        IProfessionalRepository professionalRepository,
        IProfessionalTransactionRepository transactionRepository,
        IProfessionalPaymentService paymentService,
        IFogTripTicketCheckoutEmailService checkoutEmailService)
        : base(authService, professionalRepository, transactionRepository, paymentService)
    {
        _mapper = mapper;
        _ticketRepository = ticketRepository;
        _siteRepository = siteRepository;
        _checkoutEmailService = checkoutEmailService;
    }

    protected override List<CheckoutItemDto> GetItems(FogTripTicketCheckoutRequestDto request)
    {
        return request.Tickets;
    }

    protected override Task<List<FogTripTicket>> GetUnpaidItemsAsync(List<int> ids, CancellationToken cancellationToken)
    {
        return _ticketRepository.GetUnpaidForCheckoutAsync(ids, AuthService.ProfessionalId, GetTransporterId(), cancellationToken);
    }

    protected override Task<int> MarkItemsPaidAsync(List<FogTripTicket> tickets, ProfessionalTransaction transaction, List<int> emailPdfIds)
    {
        var ticketIds = tickets.Select(ticket => ticket.Id).ToList();

        return _ticketRepository.MarkPaidAsync(
            ticketIds, transaction.ProfessionalId, GetTransporterId(), transaction.TransactionId!, transaction.TransactionDate, emailPdfIds, CancellationToken.None);
    }

    protected override Task<decimal> SumPaidAmountAsync(ProfessionalTransaction transaction)
    {
        return _ticketRepository.SumAmountByTransactionIdAsync(transaction.TransactionId!, transaction.ProfessionalId, CancellationToken.None);
    }

    protected override Task OnItemsPaidAsync(List<FogTripTicket> tickets)
    {
        var lastTripTicketDates = tickets
            .Where(ticket => ticket.InterceptorWasteRemovedDate != null)
            .GroupBy(ticket => ticket.SiteId)
            .ToDictionary(site => site.Key, site => site.Max(ticket => ticket.InterceptorWasteRemovedDate!.Value.Date));

        return _siteRepository.UpdateLastTripTicketDatesAsync(lastTripTicketDates);
    }

    protected override async Task<FogTripTicketCheckoutReceiptDto> BuildReceiptAsync(ProfessionalTransaction transaction, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetByTransactionIdAsync(transaction.TransactionId!, transaction.ProfessionalId, cancellationToken);
        var receipt = CreateReceipt(transaction);

        receipt.Tickets = _mapper.Map<List<FogTripTicketDto>>(tickets);

        return receipt;
    }

    protected override async Task OnPaymentCompletedAsync(FogTripTicketCheckoutRequestDto request, FogTripTicketCheckoutReceiptDto receipt)
    {
        var emailPdfTicketIds = request.Tickets.Where(ticket => ticket.EmailPdf).Select(ticket => ticket.Id).ToHashSet();
        var ticketsToEmail = receipt.Tickets.Where(ticket => emailPdfTicketIds.Contains(ticket.Id));

        receipt.EmailResults = await _checkoutEmailService.SendTripTicketReportsAsync(ticketsToEmail, request.TransactionId);
    }

    private int? GetTransporterId()
    {
        return AuthService.HasAnyRole(RoleDefinitions.Professionals.Admin) ? null : AuthService.UserId;
    }
}
