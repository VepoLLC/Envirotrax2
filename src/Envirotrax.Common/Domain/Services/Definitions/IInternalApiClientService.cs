
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IInternalApiClientService : IInternalApiClientService<InternalApiOptions>
{

}

public interface IInternalApiClientService<TOptions>
    where TOptions : InternalApiOptions
{
    Task<T?> GetAsync<T>(int waterSupplierId, int? loggedInUserId, string url);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, ServiceMessageDto<TRequest> requestData);

    Task<TResponse?> PutAsync<TRequest, TResponse>(string url, ServiceMessageDto<TRequest> requestData);

    Task<T?> DeleteAsync<T>(int waterSupplierId, int? loggedInUserId, string url);
}

