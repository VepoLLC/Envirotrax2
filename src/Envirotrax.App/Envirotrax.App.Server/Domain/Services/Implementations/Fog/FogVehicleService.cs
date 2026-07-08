using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogVehicleService : Service<FogVehicle, FogVehicleDto>, IFogVehicleService
{
    public FogVehicleService(IMapper mapper, IFogVehicleRepository repository)
        : base(mapper, repository)
    {
    }
}
