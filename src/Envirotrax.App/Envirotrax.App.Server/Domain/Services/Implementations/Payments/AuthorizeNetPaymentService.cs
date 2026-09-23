
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Envirotrax.App.Server.Domain.Configuration;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Microsoft.Extensions.Options;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Payments;

public class AuthorizeNetPaymentService : IAuthorizeNetPaymentService
{
    private const string ApprovedResponseCode = "1";

    private readonly HttpClient _http;
    private readonly AuthorizeNetOptions _options;

    public AuthorizeNetPaymentService(HttpClient http, IOptions<AuthorizeNetOptions> options)
    {
        _http = http;
        _options = options.Value;

        _http.BaseAddress = new(_options.BaseUrl);
    }

    public async Task<AuthorizeNetChargeResult> ChargeAsync(string dataDescriptor, string dataValue, decimal amount, string invoiceNumber, AuthorizeNetBillingInfo billingInfo, CancellationToken cancellationToken)
    {
        var transactionRequest = new
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
            order = new
            {
                invoiceNumber
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
        };

        var response = await SendTransactionAsync(transactionRequest, cancellationToken);

        if (response.TransactionResponse?.ResponseCode == ApprovedResponseCode)
        {
            return new AuthorizeNetChargeResult
            {
                IsApproved = true,
                TransactionId = response.TransactionResponse.TransId,
                CardNumber = response.TransactionResponse.AccountNumber,
                CardType = response.TransactionResponse.AccountType
            };
        }

        return new AuthorizeNetChargeResult
        {
            IsApproved = false,
            ErrorMessage = GetErrorMessage(response) ?? "The card was declined."
        };
    }

    public async Task<bool> VoidAsync(string gatewayTransactionId, CancellationToken cancellationToken)
    {
        var transactionRequest = new
        {
            transactionType = "voidTransaction",
            refTransId = gatewayTransactionId
        };

        var response = await SendTransactionAsync(transactionRequest, cancellationToken);

        return response.TransactionResponse?.ResponseCode == ApprovedResponseCode;
    }

    private async Task<TransactionResponseEnvelope> SendTransactionAsync(object transactionRequest, CancellationToken cancellationToken)
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
                transactionRequest
            }
        };

        var httpResponse = await _http.PostAsJsonAsync(string.Empty, request, cancellationToken);
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Authorize.Net request failed with status {(int)httpResponse.StatusCode}. Response: {json}");
        }

        // Authorize.Net's JSON API prepends a UTF-8 BOM to the response body, which breaks strict JSON parsing.
        json = json.TrimStart((char)0xFEFF);

        return JsonSerializer.Deserialize<TransactionResponseEnvelope>(json)
            ?? throw new InvalidOperationException($"Unable to parse Authorize.Net response: {json}");
    }

    private static string? GetErrorMessage(TransactionResponseEnvelope response)
    {
        return response.TransactionResponse?.Errors?.FirstOrDefault()?.ErrorText
            ?? response.Messages?.Message?.FirstOrDefault()?.Text;
    }

    class TransactionResponseEnvelope
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

        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("accountType")]
        public string? AccountType { get; set; }

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
