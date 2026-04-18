using AutoMapper;
using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;
using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;

namespace Envirotrax.App.Server.Domain.Services.Implementations.GisAreas;

public class GisAreaService : Service<GisArea, GisAreaDto>, IGisAreaService
{
    public GisAreaService(IMapper mapper, IGisAreaRepository repository)
        : base(mapper, repository)
    {
    }
}
