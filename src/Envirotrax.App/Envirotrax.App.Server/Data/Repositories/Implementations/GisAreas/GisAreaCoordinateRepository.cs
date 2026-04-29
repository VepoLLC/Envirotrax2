using System.Transactions;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.GisAreas;

public class GisAreaCoordinateRepository : Repository<GisAreaCoordinate, long>, IGisAreaCoordinateRepository
{
    public GisAreaCoordinateRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<GisAreaCoordinate>> GetByAreaIdAsync(int areaId, CancellationToken cancellationToken)
    {
        return await DbContext.GisAreaCoordinates
            .AsNoTracking()
            .Where(c => c.AreaId == areaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<GisAreaCoordinate>> AddOrUpdateAsync(int areaId, IEnumerable<GisAreaCoordinate> coordinates)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await DeleteByAreaAsyc(areaId);

            DbContext.GisAreaCoordinates.AddRange(coordinates);
            await DbContext.SaveChangesAsync();

            scope.Complete();
            return coordinates;
        }
    }

    public async Task DeleteByAreaAsyc(int areaId)
    {
        await DbContext
            .GisAreaCoordinates
            .Where(c => c.AreaId == areaId)
            .ExecuteDeleteAsync();
    }
}