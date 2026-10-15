
using System.Dynamic;
using System.Text.Json;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class EmailService : IEmailService
{
    private static readonly TimeSpan MaxScheduleAhead = TimeSpan.FromHours(72);

    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;
    private readonly IHtmlTemplateService _templateService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IHostEnvironment _environment;

    private readonly ISendGridClient _emailClient;

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

        _emailClient = new SendGridClient(_emailOptions.ApiKey);
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
            var message = await BuildMessageAsync(email);

            await SendMessageAsync(message);
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

    public async Task<string> CreateBatchIdAsync()
    {
        var response = await _emailClient.RequestAsync(method: BaseClient.Method.POST, urlPath: "mail/batch");

        await EnsureSuccessAsync(response);

        var responseBody = await response.Body.ReadAsStringAsync();

        using var document = JsonDocument.Parse(responseBody);

        return document.RootElement.GetProperty("batch_id").GetString()!;
    }

    public async Task ScheduleAsync<TTemplate>(EmailDto<TTemplate> email, string batchId, DateTimeOffset sendAt)
    {
        if (sendAt > DateTimeOffset.UtcNow.Add(MaxScheduleAhead))
        {
            throw new ArgumentOutOfRangeException(nameof(sendAt), $"SendGrid cannot schedule an email more than {MaxScheduleAhead.TotalHours} hours ahead.");
        }

        var message = await BuildMessageAsync(email);

        message.BatchId = batchId;
        message.SendAt = sendAt.ToUnixTimeSeconds();

        await SendMessageAsync(message);
    }

    private async Task<SendGridMessage> BuildMessageAsync<TTemplate>(EmailDto<TTemplate> email)
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

        var message = new SendGridMessage
        {
            From = new EmailAddress(fromAddress),
            Subject = email.Subject ?? string.Empty,
            HtmlContent = body,
        };

        message.AddTos(GetToAddresses(email.Recipients).Select(address => new EmailAddress(address)).ToList());

        foreach (var attachment in email.Attachments)
        {
            message.AddAttachment(attachment.Name, Convert.ToBase64String(attachment.Content), attachment.ContentType);
        }

        return message;
    }

    private async Task SendMessageAsync(SendGridMessage message)
    {
        var response = await _emailClient.SendEmailAsync(message);

        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(Response response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid returned {(int)response.StatusCode}: {responseBody}");
        }
    }
}