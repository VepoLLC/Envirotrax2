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
