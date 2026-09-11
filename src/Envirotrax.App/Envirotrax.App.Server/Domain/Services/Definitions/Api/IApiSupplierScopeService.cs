using Envirotrax.App.Server.Data.Models.Api;
using Envirotrax.App.Server.Domain.DataTransferObjects.Api;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Api;

public interface IApiSupplierScopeService
{
    Task<ApiSupplierScope?> ResolveAsync(ApiAccount account, CancellationToken cancellationToken);
}
