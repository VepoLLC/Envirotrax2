
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Options;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class InternalApiClientService : InternalApiClientService<InternalApiOptions>, IInternalApiClientService
{
    public InternalApiClientService(IHttpClientFactory httpClientFactory, IOptions<InternalApiOptions> apiOptions)
        : base(httpClientFactory, apiOptions)
    {
    }
}

public class InternalApiClientService<TOptions> : IInternalApiClientService<TOptions>
    where TOptions : InternalApiOptions
{
    private readonly HttpClient _httpClient;
    private readonly TOptions _apiOptions;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    private static TokenResponse? _tokenResponse;
    private static DateTime? _tokenIssuedTime;

    public InternalApiClientService(IHttpClientFactory httpClientFactory, IOptions<TOptions> apiOptions)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiOptions = apiOptions.Value;

        _httpClient.BaseAddress = new(_apiOptions.BaseUrl);
    }

    private async Task SetTokenAsync()
    {
        if (_tokenResponse is null || (_tokenIssuedTime.HasValue && DateTime.UtcNow > _tokenIssuedTime.Value.AddSeconds(_tokenResponse.ExpiresIn - 60)))
        {
            var response = await _httpClient.PostAsync(_apiOptions.TokenEndpoint, new FormUrlEncodedContent(
            [
                new("grant_type", "client_credentials"),
                new("client_id", _apiOptions.ClientId),
                new("client_secret", _apiOptions.ClientSecret),
                new("scope", _apiOptions.Scope.Trim()),
            ]));

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(content);
            }

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>() ?? throw new InvalidOperationException();

            _tokenResponse = token;
            _tokenIssuedTime = DateTime.UtcNow;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", _tokenResponse.AccessToken);
    }

    private HttpRequestMessage CreateRequestMessage(HttpMethod method, int waterSupplierId, int? loggedInUserId, string url)
    {
        var request = new HttpRequestMessage(method, url);

        request.Headers.Add("Vp-Water-Supplier-Id", waterSupplierId.ToString());

        if (loggedInUserId.HasValue)
        {
            request.Headers.Add("Vp-User-Id", loggedInUserId.Value.ToString());
        }

        return request;
    }

    private async Task<T?> ProcessRequestAsync<T>(Func<Task<HttpResponseMessage>> requestCallback)
    {
        await SetTokenAsync();

        var response = await requestCallback();

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await SetTokenAsync();
            response = await requestCallback();
        }

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(content);
        }

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    public Task<T?> GetAsync<T>(int waterSupplierId, int? loggedInUserId, string url)
    {
        return ProcessRequestAsync<T>(() =>
        {
            var request = CreateRequestMessage(HttpMethod.Get, waterSupplierId, loggedInUserId, url);
            return _httpClient.SendAsync(request);
        });
    }

    public Task<TResponse?> PostAsync<TRequest, TResponse>(string url, ServiceMessageDto<TRequest> requestData)
    {
        return ProcessRequestAsync<TResponse>(() =>
        {
            var request = CreateRequestMessage(HttpMethod.Post, requestData.WaterSupplierId, requestData.LoggedInUserId, url);

            if (requestData.Data is not null)
            {
                var json = JsonSerializer.Serialize(requestData.Data, _jsonOptions);
                request.Content = new StringContent(json, MediaTypeHeaderValue.Parse("application/json"));
            }

            return _httpClient.SendAsync(request);
        });
    }

    public Task<TResponse?> PutAsync<TRequest, TResponse>(string url, ServiceMessageDto<TRequest> requestData)
    {
        return ProcessRequestAsync<TResponse>(() =>
        {
            var request = CreateRequestMessage(HttpMethod.Put, requestData.WaterSupplierId, requestData.LoggedInUserId, url);

            if (requestData.Data is not null)
            {
                var json = JsonSerializer.Serialize(requestData.Data, _jsonOptions);
                request.Content = new StringContent(json, MediaTypeHeaderValue.Parse("application/json"));
            }

            return _httpClient.SendAsync(request);
        });
    }

    public Task<T?> DeleteAsync<T>(int waterSupplierId, int? loggedInUserId, string url)
    {
        return ProcessRequestAsync<T>(() =>
        {
            var request = CreateRequestMessage(HttpMethod.Post, waterSupplierId, loggedInUserId, url);

            return _httpClient.SendAsync(request);
        });
    }
}

class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = null!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}