using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Templates.Emails.Fog;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogTripTicketCheckoutEmailService : IFogTripTicketCheckoutEmailService
{
    private readonly IAuthService _authService;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IFogTripTicketService _tripTicketService;
    private readonly IEmailService _emailService;
    private readonly ILogger<FogTripTicketCheckoutEmailService> _logger;

    public FogTripTicketCheckoutEmailService(
        IAuthService authService,
        IProfessionalUserRepository professionalUserRepository,
        IFogTripTicketService tripTicketService,
        IEmailService emailService,
        ILogger<FogTripTicketCheckoutEmailService> logger)
    {
        _authService = authService;
        _professionalUserRepository = professionalUserRepository;
        _tripTicketService = tripTicketService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<List<CheckoutEmailResultDto>> SendTripTicketReportsAsync(IEnumerable<FogTripTicketDto> tickets, string transactionId)
    {
        var ticketsBySite = tickets.GroupBy(ticket => ticket.Site?.Id).ToList();

        if (ticketsBySite.Count == 0)
        {
            return [];
        }

        var payer = await _professionalUserRepository.GetAsync(_authService.UserId, CancellationToken.None);
        var recipient = payer?.User?.Email;

        if (string.IsNullOrWhiteSpace(recipient))
        {
            _logger.LogWarning("Payment {TransactionId} has trip tickets to email, but user {UserId} has no email address.", transactionId, _authService.UserId);
            return [];
        }

        var results = new List<CheckoutEmailResultDto>();

        foreach (var siteTickets in ticketsBySite)
        {
            results.Add(await SendSiteReportAsync(recipient, [.. siteTickets], transactionId));
        }

        return results;
    }

    private async Task<CheckoutEmailResultDto> SendSiteReportAsync(string recipient, List<FogTripTicketDto> siteTickets, string transactionId)
    {
        var vm = BuildEmailVm(siteTickets);
        var description = string.IsNullOrWhiteSpace(vm.PropertyBusinessName)
            ? vm.PropertyAddress
            : $"{vm.PropertyBusinessName}: {vm.PropertyAddress}";

        try
        {
            var pdf = await _tripTicketService.GeneratePdfWithSignaturesAsync(siteTickets);

            await _emailService.SendAsync(new EmailDto<FogTripTicketCheckoutVm>
            {
                Recipients = [recipient],
                Subject = $"Envirotrax Trip Ticket - {description}",
                TemplateId = "Fog.FogTripTicketCheckout",
                TemplateData = vm,
                Attachments =
                [
                    new EmailAttachmentDto
                    {
                        Name = $"{transactionId}-{siteTickets[0].Site?.Id ?? siteTickets[0].Id}.pdf",
                        ContentType = "application/pdf",
                        Content = pdf
                    }
                ]
            });

            return new CheckoutEmailResultDto { Description = description, IsSent = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to email trip tickets for payment {TransactionId}.", transactionId);

            return new CheckoutEmailResultDto { Description = description, IsSent = false };
        }
    }

    private static FogTripTicketCheckoutVm BuildEmailVm(List<FogTripTicketDto> siteTickets)
    {
        var property = siteTickets[0];

        return new FogTripTicketCheckoutVm
        {
            PropertyBusinessName = property.PropertyBusinessName,
            PropertyAddress = JoinNonEmpty(" ", property.PropertyStreetNumber, property.PropertyStreetName,
                string.IsNullOrWhiteSpace(property.PropertyNumber) ? null : $"#{property.PropertyNumber}"),
            PropertyCityStateZip = JoinNonEmpty(", ", property.PropertyCity, JoinNonEmpty(" ", property.PropertyState?.Code, property.PropertyZip)),
            Tickets = [.. siteTickets.Select(ticket => new FogTripTicketCheckoutItemVm
            {
                TransporterName = ticket.TransporterContactName,
                Vehicle = JoinNonEmpty(" ", ticket.VehicleYear > 0 ? ticket.VehicleYear.ToString() : null, ticket.VehicleManufacturer, ticket.VehicleLicensePlateNumber),
                WasteRemoved = $"{ticket.InterceptorWasteRemovedAmount} {GetCapacityLabel(ticket.InterceptorWasteRemovedType)}",
                WasteRemovedDate = ticket.InterceptorWasteRemovedDate?.ToString("MM/dd/yyyy h:mm tt"),
                Receiver = ticket.ReceiverCompanyName
            })]
        };
    }

    private static string GetCapacityLabel(FogVehicleCapacityType capacityType)
    {
        return capacityType == FogVehicleCapacityType.CubicYards ? "Cubic Yards" : "Gallons";
    }

    private static string JoinNonEmpty(string separator, params string?[] parts)
    {
        return string.Join(separator, parts.Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}
