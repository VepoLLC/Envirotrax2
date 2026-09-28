using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Templates.Emails.Csi;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiCheckoutEmailService : ICsiCheckoutEmailService
{
    private readonly IAuthService _authService;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly ICsiInspectionService _csiInspectionService;
    private readonly IEmailService _emailService;
    private readonly ILogger<CsiCheckoutEmailService> _logger;

    public CsiCheckoutEmailService(
        IAuthService authService,
        IProfessionalUserRepository professionalUserRepository,
        ICsiInspectionService csiInspectionService,
        IEmailService emailService,
        ILogger<CsiCheckoutEmailService> logger)
    {
        _authService = authService;
        _professionalUserRepository = professionalUserRepository;
        _csiInspectionService = csiInspectionService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<List<CheckoutEmailResultDto>> SendInspectionReportsAsync(IEnumerable<CsiInspectionDto> inspections, string transactionId)
    {
        var inspectionsBySite = inspections.GroupBy(inspection => inspection.Site?.Id).ToList();

        if (inspectionsBySite.Count == 0)
        {
            return [];
        }

        var payer = await _professionalUserRepository.GetAsync(_authService.UserId, CancellationToken.None);
        var recipient = payer?.User?.Email;

        if (string.IsNullOrWhiteSpace(recipient))
        {
            _logger.LogWarning("Payment {TransactionId} has inspection reports to email, but user {UserId} has no email address.", transactionId, _authService.UserId);
            return [];
        }

        var results = new List<CheckoutEmailResultDto>();

        foreach (var siteInspections in inspectionsBySite)
        {
            results.Add(await SendSiteReportAsync(recipient, [.. siteInspections], transactionId));
        }

        return results;
    }

    private async Task<CheckoutEmailResultDto> SendSiteReportAsync(string recipient, List<CsiInspectionDto> siteInspections, string transactionId)
    {
        var vm = BuildEmailVm(siteInspections);
        var description = string.IsNullOrWhiteSpace(vm.PropertyBusinessName)
            ? vm.PropertyAddress
            : $"{vm.PropertyBusinessName}: {vm.PropertyAddress}";

        try
        {
            var pdf = await _csiInspectionService.GeneratePdfAsync(siteInspections);

            await _emailService.SendAsync(new EmailDto<CsiInspectionCheckoutVm>
            {
                Recipients = [recipient],
                Subject = $"Envirotrax CSI Inspection - {description}",
                TemplateId = "Csi.CsiInspectionCheckout",
                TemplateData = vm,
                Attachments =
                [
                    new EmailAttachmentDto
                    {
                        Name = $"{transactionId}-{siteInspections[0].Site?.Id ?? siteInspections[0].Id}.pdf",
                        ContentType = "application/pdf",
                        Content = pdf
                    }
                ]
            });

            return new CheckoutEmailResultDto { Description = description, IsSent = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to email CSI inspection reports for payment {TransactionId}.", transactionId);

            return new CheckoutEmailResultDto { Description = description, IsSent = false };
        }
    }

    private static CsiInspectionCheckoutVm BuildEmailVm(List<CsiInspectionDto> siteInspections)
    {
        var property = siteInspections[0];

        return new CsiInspectionCheckoutVm
        {
            PropertyBusinessName = property.PropertyBusinessName,
            PropertyAddress = JoinNonEmpty(" ", property.PropertyStreetNumber, property.PropertyStreetName,
                string.IsNullOrWhiteSpace(property.PropertyNumber) ? null : $"#{property.PropertyNumber}"),
            PropertyCityStateZip = JoinNonEmpty(", ", property.PropertyCity, JoinNonEmpty(" ", property.PropertyState?.Code, property.PropertyZip)),
            Inspections = [.. siteInspections.Select(inspection => new CsiInspectionCheckoutItemVm
            {
                InspectionDate = inspection.InspectionDate?.ToString("MM/dd/yyyy"),
                InspectedBy = inspection.InspectorContactName
            })]
        };
    }

    private static string JoinNonEmpty(string separator, params string?[] parts)
    {
        return string.Join(separator, parts.Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}
