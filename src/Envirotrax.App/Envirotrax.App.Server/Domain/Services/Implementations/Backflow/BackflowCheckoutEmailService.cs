using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Templates.Emails.Backflow;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowCheckoutEmailService : IBackflowCheckoutEmailService
{
    private readonly IAuthService _authService;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IBackflowTestService _backflowTestService;
    private readonly IEmailService _emailService;
    private readonly ILogger<BackflowCheckoutEmailService> _logger;

    public BackflowCheckoutEmailService(
        IAuthService authService,
        IProfessionalUserRepository professionalUserRepository,
        IBackflowTestService backflowTestService,
        IEmailService emailService,
        ILogger<BackflowCheckoutEmailService> logger)
    {
        _authService = authService;
        _professionalUserRepository = professionalUserRepository;
        _backflowTestService = backflowTestService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<List<BackflowCheckoutEmailResultDto>> SendTestReportsAsync(IEnumerable<BackflowTestDto> tests, string transactionId)
    {
        var testsBySite = tests.GroupBy(test => test.Site?.Id).ToList();

        if (testsBySite.Count == 0)
        {
            return [];
        }

        var payer = await _professionalUserRepository.GetAsync(_authService.UserId, CancellationToken.None);
        var recipient = payer?.User?.Email;

        if (string.IsNullOrWhiteSpace(recipient))
        {
            _logger.LogWarning("Payment {TransactionId} has test reports to email, but user {UserId} has no email address.", transactionId, _authService.UserId);
            return [];
        }

        var results = new List<BackflowCheckoutEmailResultDto>();

        foreach (var siteTests in testsBySite)
        {
            results.Add(await SendSiteReportAsync(recipient, [.. siteTests], transactionId));
        }

        return results;
    }

    private async Task<BackflowCheckoutEmailResultDto> SendSiteReportAsync(string recipient, List<BackflowTestDto> siteTests, string transactionId)
    {
        var vm = BuildEmailVm(siteTests);
        var description = string.IsNullOrWhiteSpace(vm.PropertyBusinessName)
            ? vm.PropertyAddress
            : $"{vm.PropertyBusinessName}: {vm.PropertyAddress}";

        try
        {
            var pdf = await _backflowTestService.GeneratePdfAsync(siteTests);

            await _emailService.SendAsync(new EmailDto<BackflowTestCheckoutVm>
            {
                Recipients = [recipient],
                Subject = $"Envirotrax Backflow Test - {description}",
                TemplateId = "Backflow.BackflowTestCheckout",
                TemplateData = vm,
                Attachments =
                [
                    new EmailAttachmentDto
                    {
                        Name = $"{transactionId}-{siteTests[0].Site?.Id ?? siteTests[0].Id}.pdf",
                        ContentType = "application/pdf",
                        Content = pdf
                    }
                ]
            });

            return new BackflowCheckoutEmailResultDto { Description = description, IsSent = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to email backflow test reports for payment {TransactionId}.", transactionId);

            return new BackflowCheckoutEmailResultDto { Description = description, IsSent = false };
        }
    }

    private static BackflowTestCheckoutVm BuildEmailVm(List<BackflowTestDto> siteTests)
    {
        var property = siteTests[0];

        return new BackflowTestCheckoutVm
        {
            PropertyBusinessName = property.PropertyBusinessName,
            PropertyAddress = JoinNonEmpty(" ", property.PropertyStreetNumber, property.PropertyStreetName,
                string.IsNullOrWhiteSpace(property.PropertyNumber) ? null : $"#{property.PropertyNumber}"),
            PropertyCityStateZip = JoinNonEmpty(", ", property.PropertyCity, JoinNonEmpty(" ", property.PropertyState?.Code, property.PropertyZip)),
            Tests = [.. siteTests.Select(test => new BackflowTestCheckoutItemVm
            {
                TestDate = test.TestDate?.ToString("MM/dd/yyyy"),
                AssemblyDescription = JoinNonEmpty(" ", test.Manufacturer, test.Model, test.Size, test.DeviceType),
                SerialNumber = test.SerialNumber,
                TestedBy = test.BpatContactName
            })]
        };
    }

    private static string JoinNonEmpty(string separator, params string?[] parts)
    {
        return string.Join(separator, parts.Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}
