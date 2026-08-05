using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<FogVehicle>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await DbContext.FogVehicles
            .AsNoTracking()
            .Where(v => v.ProfessionalId == professionalId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
