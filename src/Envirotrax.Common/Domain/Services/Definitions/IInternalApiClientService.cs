
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IInternalApiClientService : IInternalApiClientService<InternalApiOptions>
{

}

public interface IInternalApiClientService<TOptions>
    where TOptions : InternalApiOptions
{
    Task<T?> GetAsync<T>(int? loggedInUserId, string url, CancellationToken cancellationToken);

    Task<T?> GetAsync<T>(int waterSupplierId, int? loggedInUserId, string url, CancellationToken cancellationToken);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, ServiceMessageDto<TRequest> requestData, CancellationToken cancellationToken);

    Task<TResponse?> PutAsync<TRequest, TResponse>(string url, ServiceMessageDto<TRequest> requestData, CancellationToken cancellationToken);

    Task<T?> DeleteAsync<T>(int? loggedInUserId, string url, CancellationToken cancellationToken);
    Task<T?> DeleteAsync<T>(int waterSupplierId, int? loggedInUserId, string url, CancellationToken cancellationToken);
}

