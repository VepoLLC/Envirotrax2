
using DeveloperPartners.SortingFiltering;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions;

public interface IEnvirotraxApiClient
{
    Task<TResponse?> GetAsync<TResponse>(string url);
    Task<IPagedData<TResponse>> GetAsync<TResponse>(string url, PageInfo pageInfo, Query query);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest requestData);

    Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest requestData);

    Task<TResponse?> DeleteAsync<TResponse>(string url);
}