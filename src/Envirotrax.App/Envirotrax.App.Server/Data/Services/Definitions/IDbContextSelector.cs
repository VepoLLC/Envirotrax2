
using Envirotrax.App.Server.Data.DbContexts;

namespace Envirotrax.App.Server.Data.Services.Definitions
{
    public interface IDbContextSelector
    {
        TenantDbContext Current { get; }
    }
}