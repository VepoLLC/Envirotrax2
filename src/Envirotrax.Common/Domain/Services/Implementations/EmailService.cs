
using System.Net;
using System.Net.Mail;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class EmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;
    private readonly IHtmlTemplateService _templateService;

    public EmailService(
        IOptions<EmailOptions> emailOptions,
        ILogger<EmailService> logger,
        IHtmlTemplateService templateService)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
        _templateService = templateService;
    }

    private MailAddress GetFromAddress(FromAddressType addressType)
    {
        switch (addressType)
        {
            case FromAddressType.Team:
                return new("Envirotrax.com<team@envirotrax.com>");
            case FromAddressType.Info:
                return new("Envirotrax.com<info@envirotrax.com>");
            default:
                return new("Envirotrax.com<noreply@envirotrax.com>");
        }
    }

    private IEnumerable<string> GetToAddresses(IEnumerable<string> recipients)
    {
        if (!string.IsNullOrWhiteSpace(_emailOptions.OverrideRecipients))
        {
            return _emailOptions.OverrideRecipients.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        return recipients;
    }

    public async Task SendAsync<TTemplate>(EmailDto email)
    {
        try
        {
            var mail = new MailMessage
            {
                From = GetFromAddress(email.FromAddress),
                Subject = email.Subject,
                IsBodyHtml = true
            };

            if (!string.IsNullOrWhiteSpace(email.TemplateId))
            {
                mail.Body = await _templateService.ParseEmailAsync(email.TemplateId, email.TemplateData);
            }

            foreach (var address in GetToAddresses(email.Recipients))
            {
                mail.To.Add(address);
            }

            using (var smtp = new SmtpClient(_emailOptions.Host, _emailOptions.Port))
            {
                smtp.Credentials = new NetworkCredential(_emailOptions.Username, _emailOptions.Password);
                await smtp.SendMailAsync(mail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email.");
        }
    }
}