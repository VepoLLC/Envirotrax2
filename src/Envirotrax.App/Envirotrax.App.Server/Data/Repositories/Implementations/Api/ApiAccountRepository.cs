using Envirotrax.App.Server.Data.Models.Api;
using Envirotrax.App.Server.Data.Repositories.Definitions.Api;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Api;

public class ApiAccountRepository : Repository<ApiAccount>, IApiAccountRepository
{
    public ApiAccountRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<ApiAccount?> GetByCredentialsAsync(string userId, string apiKeyHash, CancellationToken cancellationToken)
    {
        return await GetDetailsQuery()
            .Where(account => account.UserId == userId)
            .Where(account => account.ApiKeyHash == apiKeyHash)
            .Where(account => account.ApiKeyHash != "")
            .FirstOrDefaultAsync(cancellationToken);
    }
}
