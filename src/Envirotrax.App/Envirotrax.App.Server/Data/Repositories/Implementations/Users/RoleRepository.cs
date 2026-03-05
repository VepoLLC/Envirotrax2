
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Users;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}