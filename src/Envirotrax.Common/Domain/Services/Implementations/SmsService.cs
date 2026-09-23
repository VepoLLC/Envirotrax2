
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly SmsOptions _smsOptions;
    private readonly ILogger<SmsService> _logger;
    private readonly IHostEnvironment _environment;

    public SmsService(
        IHttpClientFactory httpClientFactory,
        IOptions<SmsOptions> smsOptions,
        ILogger<SmsService> logger,
        IHostEnvironment environment)
    {
        _httpClient = httpClientFactory.CreateClient();
        _smsOptions = smsOptions.Value;
        _logger = logger;
        _environment = environment;

        _httpClient.BaseAddress = new("https://rest.clicksend.com/v3/");

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_smsOptions.Username}:{_smsOptions.ApiKey}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    }

    public async Task SendAsync(SmsDto sms)
    {
        try
        {
            var to = string.IsNullOrWhiteSpace(_smsOptions.OverrideRecipient) ? sms.To : _smsOptions.OverrideRecipient;

            var payload = new
            {
                messages = new[]
                {
                    new
                    {
                        source = "sdk",
                        body = sms.Body,
                        to
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("sms/send", payload);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"ClickSend SMS API call failed with {response.StatusCode}. Content: {content}");
            }
        }
        catch (Exception ex)
        {
            if (_environment.IsDevelopment())
            {
                throw;
            }
            else
            {
                _logger.LogError(ex, "Error sending SMS.");
            }
        }
    }
}
