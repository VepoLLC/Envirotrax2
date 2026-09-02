
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Envirotrax.App.Server.Domain.Configuration;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Microsoft.Extensions.Options;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Payments;

public class AuthorizeNetPaymentService : IAuthorizeNetPaymentService
{
    private readonly HttpClient _http;
    private readonly AuthorizeNetOptions _options;

    public AuthorizeNetPaymentService(HttpClient http, IOptions<AuthorizeNetOptions> options)
    {
        _http = http;
        _options = options.Value;

        _http.BaseAddress = new(_options.BaseUrl);
    }

    public async Task<AuthorizeNetChargeResult> ChargeAsync(string dataDescriptor, string dataValue, decimal amount, AuthorizeNetBillingInfo billingInfo, CancellationToken cancellationToken)
    {
        var request = new
        {
            createTransactionRequest = new
            {
                merchantAuthentication = new
                {
                    name = _options.ApiLoginId,
                    transactionKey = _options.TransactionKey
                },
                transactionRequest = new
                {
                    transactionType = "authCaptureTransaction",
                    amount = amount.ToString("F2", CultureInfo.InvariantCulture),
                    payment = new
                    {
                        opaqueData = new
                        {
                            dataDescriptor,
                            dataValue
                        }
                    },
                    billTo = new
                    {
                        firstName = billingInfo.FirstName,
                        lastName = billingInfo.LastName,
                        address = billingInfo.Address,
                        city = billingInfo.City,
                        state = billingInfo.State,
                        zip = billingInfo.Zip
                    }
                }
            }
        };

        var httpResponse = await _http.PostAsJsonAsync(string.Empty, request, cancellationToken);
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        // Authorize.Net's JSON API prepends a UTF-8 BOM to the response body, which breaks strict JSON parsing.
        json = json.TrimStart((char)0xFEFF);

        var response = JsonSerializer.Deserialize<ChargeResponse>(json)
            ?? throw new InvalidOperationException($"Unable to parse Authorize.Net response: {json}");

        if (response.TransactionResponse?.ResponseCode == "1")
        {
            return new AuthorizeNetChargeResult
            {
                IsApproved = true,
                TransactionId = response.TransactionResponse.TransId
            };
        }

        var errorMessage = response.TransactionResponse?.Errors?.FirstOrDefault()?.ErrorText
            ?? response.Messages?.Message?.FirstOrDefault()?.Text
            ?? "The card was declined.";

        return new AuthorizeNetChargeResult
        {
            IsApproved = false,
            ErrorMessage = errorMessage
        };
    }

    class ChargeResponse
    {
        [JsonPropertyName("transactionResponse")]
        public TransactionResponse? TransactionResponse { get; set; }

        [JsonPropertyName("messages")]
        public ResponseMessages? Messages { get; set; }
    }

    class TransactionResponse
    {
        [JsonPropertyName("responseCode")]
        public string? ResponseCode { get; set; }

        [JsonPropertyName("transId")]
        public string? TransId { get; set; }

        [JsonPropertyName("errors")]
        public List<TransactionError>? Errors { get; set; }
    }

    class TransactionError
    {
        [JsonPropertyName("errorText")]
        public string? ErrorText { get; set; }
    }

    class ResponseMessages
    {
        [JsonPropertyName("message")]
        public List<ResponseMessage>? Message { get; set; }
    }

    class ResponseMessage
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
