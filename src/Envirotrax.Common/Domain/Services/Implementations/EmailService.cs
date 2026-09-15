
using System.Dynamic;
using Azure.Communication.Email;
using Azure.Identity;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;
    private readonly IHtmlTemplateService _templateService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IHostEnvironment _environment;

    private readonly EmailClient _emailClient;

    public EmailService(
        IOptions<EmailOptions> emailOptions,
        ILogger<EmailService> logger,
        IHtmlTemplateService templateService,
        IHttpContextAccessor contextAccessor,
        IHostEnvironment environment)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
        _templateService = templateService;
        _contextAccessor = contextAccessor;
        _environment = environment;

        _emailClient = new(new Uri(_emailOptions.Endpoint), new DefaultAzureCredential());
    }

    private string GetFromAddress(FromAddressType addressType)
    {
        return addressType switch
        {
            FromAddressType.Team => _emailOptions.TeamAddress,
            FromAddressType.Info => _emailOptions.InfoAddress,
            _ => _emailOptions.NoreplyAddress
        };
    }

    private IEnumerable<string> GetToAddresses(IEnumerable<string> recipients)
    {
        if (!string.IsNullOrWhiteSpace(_emailOptions.OverrideRecipients))
        {
            return _emailOptions.OverrideRecipients.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        return recipients;
    }

    public Task SendAsync(EmailDto email)
    {
        return SendAsync<object>(email);
    }

    public async Task SendAsync<TTemplate>(EmailDto<TTemplate> email)
    {
        try
        {
            var body = string.Empty;

            if (!string.IsNullOrWhiteSpace(email.TemplateId))
            {
                dynamic viewBag = new ExpandoObject();
                var request = _contextAccessor.HttpContext!.Request;

                viewBag.BaseUrl = $"https://{request.Host}";

                body = await _templateService.ParseEmailAsync(email.TemplateId, email.TemplateData, viewBag);
            }

            var fromAddress = GetFromAddress(email.FromAddress);

            var message = new EmailMessage(
                senderAddress: GetFromAddress(email.FromAddress),
                content: new EmailContent(email.Subject ?? string.Empty) { Html = body },
                recipients: new EmailRecipients(GetToAddresses(email.Recipients).Select(address => new EmailAddress(address))));

            await _emailClient.SendAsync(Azure.WaitUntil.Completed, message);
        }
        catch (Exception ex)
        {
            if (_environment.IsDevelopment())
            {
                throw;
            }
            else
            {
                _logger.LogError(ex, "Error sending email.");
            }
        }
    }
}