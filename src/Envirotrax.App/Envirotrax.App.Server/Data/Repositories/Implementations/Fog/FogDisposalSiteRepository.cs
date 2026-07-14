using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogDisposalSiteRepository : Repository<FogDisposalSite>, IFogDisposalSiteRepository
{
    public FogDisposalSiteRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}
