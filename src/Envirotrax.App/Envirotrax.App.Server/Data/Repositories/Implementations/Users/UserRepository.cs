
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Users;

public class UserRepository : Repository<WaterSupplierUser>, IUserRepository
{
    public UserRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}