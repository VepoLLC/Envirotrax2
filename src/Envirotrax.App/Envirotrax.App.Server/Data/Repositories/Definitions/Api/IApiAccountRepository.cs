using Envirotrax.App.Server.Data.Models.Api;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Api;

public interface IApiAccountRepository : IRepository<ApiAccount>
{
    Task<ApiAccount?> GetByCredentialsAsync(string userId, string apiKeyHash, CancellationToken cancellationToken);
}
