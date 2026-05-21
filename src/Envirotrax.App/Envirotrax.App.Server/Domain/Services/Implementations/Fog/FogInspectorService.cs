using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog
{
    public class FogInspectorService : Service<Professional, ProfessionalDto>, IFogInspectorService
    {
        public FogInspectorService(IMapper mapper, IFogInspectorRepository repository)
            : base(mapper, repository)
        {
        }
    }
}
