
using DeveloperPartners.SortingFiltering;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions;

public interface IEnvirotraxApiClient
{
    Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken cancellationToken);
    Task<IPagedData<TResponse>> GetAsync<TResponse>(string url, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IPagedData<TResponse>> GetAsync<TResponse>(string url, PageInfo pageInfo, Query query, IDictionary<string, string> additionalParameters, CancellationToken cancellationToken);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest requestData, CancellationToken cancellationToken);

    Task<TResponse?> PostFileAsync<TResponse>(string url, Stream fileStream, string fileName, string? description, CancellationToken cancellationToken);

    Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest requestData, CancellationToken cancellationToken);

    Task<TResponse?> DeleteAsync<TResponse>(string url, CancellationToken cancellationToken);
}