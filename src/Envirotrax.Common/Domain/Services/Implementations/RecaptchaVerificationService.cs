
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Options;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class RecaptchaVerificationService : IRecaptchaVerificationService
{
    private readonly HttpClient _httpClient;
    private readonly RecaptchaOptions _recaptchaOptions;

    public RecaptchaVerificationService(IHttpClientFactory httpClientFactory, IOptions<RecaptchaOptions> recaptchaOptions)
    {
        _httpClient = httpClientFactory.CreateClient();
        _recaptchaOptions = recaptchaOptions.Value;

        _httpClient.BaseAddress = new("https://www.google.com/recaptcha/api/");
    }

    public async Task<bool> VerifyAsync(string token, string? remoteIp = null)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var parameters = new Dictionary<string, string>
        {
            ["secret"] = _recaptchaOptions.SecretKey,
            ["response"] = token
        };

        if (!string.IsNullOrWhiteSpace(remoteIp))
        {
            parameters["remoteip"] = remoteIp;
        }

        var response = await _httpClient.PostAsync("siteverify", new FormUrlEncodedContent(parameters));

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"reCAPTCHA siteverify call failed with {response.StatusCode}. Content: {content}");
        }

        var result = await response.Content.ReadFromJsonAsync<RecaptchaSiteVerifyResponse>();

        return result?.Success ?? false;
    }

    private class RecaptchaSiteVerifyResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public string? ChallengeTimestamp { get; set; }

        [JsonPropertyName("hostname")]
        public string? Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }
    }
}
