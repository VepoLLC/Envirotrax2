
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.Configuration;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations;

public class EnvirotraxApiClient : IEnvirotraxApiClient
{
    private readonly IQueryHelperService _queryHelper;
    private readonly IAuthService _authService;
    private readonly IInternalApiClientService<EnvirotraxApiOptions> _apiClient;

    public EnvirotraxApiClient(
        IQueryHelperService queryHelper,
        IAuthService authService,
        IInternalApiClientService<EnvirotraxApiOptions> apiClient)
    {
        _queryHelper = queryHelper;
        _authService = authService;
        _apiClient = apiClient;
    }

    public Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<TResponse>(_authService.UserId, url, cancellationToken);
    }

    public Task<IPagedData<TResponse>> GetAsync<TResponse>(string url, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return GetAsync<TResponse>(url, pageInfo, query, new Dictionary<string, string>(), cancellationToken);
    }

    public async Task<IPagedData<TResponse>> GetAsync<TResponse>(string url, PageInfo pageInfo, Query query, IDictionary<string, string> additionalParameters, CancellationToken cancellationToken)
    {
        var queryString = _queryHelper.BuildQuery(pageInfo, query);

        foreach (var parameter in additionalParameters)
        {
            if (!string.IsNullOrEmpty(parameter.Value))
            {
                queryString[parameter.Key] = parameter.Value;
            }
        }

        var endpointUrl = $"{url}?{queryString}";

        return await _apiClient.GetAsync<PagedData<TResponse>>(_authService.UserId, endpointUrl, cancellationToken) ?? new PagedData<TResponse>(pageInfo, []);
    }

    public Task<TResponse?> PostAsync<TRequest, TResponse>(int waterSupplierId, string url, TRequest requestData, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<TRequest, TResponse>(url, new ServiceMessageDto<TRequest>(waterSupplierId, _authService.UserId)
        {
            Data = requestData
        }, cancellationToken);
    }

    public Task<TResponse?> PostFileAsync<TResponse>(int waterSupplierId, string url, Stream fileStream, string fileName, string? description, CancellationToken cancellationToken)
    {
        var formFields = new Dictionary<string, string>
        {
            ["description"] = description ?? string.Empty
        };

        return _apiClient.PostFileAsync<TResponse>(waterSupplierId, _authService.UserId, url, fileStream, fileName, "image", formFields, cancellationToken);
    }

    public Task<TResponse?> PostFileAsync<TResponse>(int waterSupplierId, string url, Stream fileStream, string fileName, string fileFieldName, IDictionary<string, string> formFields, CancellationToken cancellationToken)
    {
        return _apiClient.PostFileAsync<TResponse>(waterSupplierId, _authService.UserId, url, fileStream, fileName, fileFieldName, formFields, cancellationToken);
    }

    public Task<TResponse?> PutAsync<TRequest, TResponse>(int waterSupplierId, string url, TRequest requestData, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<TRequest, TResponse>(url, new ServiceMessageDto<TRequest>(waterSupplierId, _authService.UserId)
        {
            Data = requestData
        }, cancellationToken);
    }

    public Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest requestData, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<TRequest, TResponse>(url, new ServiceMessageDto<TRequest>(_authService.UserId)
        {
            Data = requestData
        }, cancellationToken);
    }

    public Task<TResponse?> DeleteAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<TResponse>(_authService.UserId, url, cancellationToken);
    }
}