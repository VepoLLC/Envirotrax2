using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogDisposalSiteService : Service<FogDisposalSite, FogDisposalSiteDto>, IFogDisposalSiteService
{
    public FogDisposalSiteService(IMapper mapper, IFogDisposalSiteRepository repository)
        : base(mapper, repository)
    {
    }
}
