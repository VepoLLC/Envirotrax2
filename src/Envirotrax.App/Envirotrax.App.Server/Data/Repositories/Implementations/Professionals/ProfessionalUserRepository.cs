
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalUserRepository : Repository<ProfessionalUser>, IProfessionalUserRepository
{
    public ProfessionalUserRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}