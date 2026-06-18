using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogVehicleRepository : Repository<FogVehicle>, IFogVehicleRepository
{
    public FogVehicleRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}
