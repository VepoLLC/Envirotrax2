
using System.Dynamic;
using System.Net;
using System.Net.Mail;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;
    private readonly IHtmlTemplateService _templateService;
    private readonly IHttpContextAccessor _contextAccessor;

    public EmailService(
        IOptions<EmailOptions> emailOptions,
        ILogger<EmailService> logger,
        IHtmlTemplateService templateService,
        IHttpContextAccessor contextAccessor)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
        _templateService = templateService;
        _contextAccessor = contextAccessor;
    }

    private MailAddress GetFromAddress(FromAddressType addressType)
    {
        switch (addressType)
        {
            case FromAddressType.Team:
                return new(_emailOptions.TeamAddress);
            case FromAddressType.Info:
                return new(_emailOptions.InfoAddress);
            default:
                return new(_emailOptions.NoreplyAddress);
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

    public Task SendAsync(EmailDto email)
    {
        return SendAsync(email);
    }

    public async Task SendAsync<TTemplate>(EmailDto<TTemplate> email)
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
                dynamic viewBag = new ExpandoObject();
                var request = _contextAccessor.HttpContext!.Request;

                viewBag.BaseUrl = $"https://{request.Host}";

                mail.Body = await _templateService.ParseEmailAsync(email.TemplateId, email.TemplateData, viewBag);
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