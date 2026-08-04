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

    // Deleting a vehicle is a soft delete (TenantDbContextBase turns EntityState.Deleted into a
    // DeletedTime stamp) and there is no global soft-delete query filter, so exclude them here —
    // otherwise deleted vehicles keep showing up in the list and in the Account Overview count.
    protected override IQueryable<FogVehicle> GetListQuery()
    {
        return base.GetListQuery()
            .Where(v => v.DeletedTime == null);
    }
}
