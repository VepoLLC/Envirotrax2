using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.GisAreas;

public class GisAreaRepository : Repository<GisArea>, IGisAreaRepository
{
    public GisAreaRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}
