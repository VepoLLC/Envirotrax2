using Envirotrax.App.Server.Data.Models.GisAreas;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;

public interface IGisAreaCoordinateRepository : IRepository<GisAreaCoordinate, long>
{
    Task<IEnumerable<GisAreaCoordinate>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<GisAreaCoordinate>> GetByAreaIdAsync(int areaId, CancellationToken cancellationToken);
    Task<IEnumerable<GisAreaCoordinate>> AddOrUpdateAsync(int areaId, IEnumerable<GisAreaCoordinate> coordinates);
    Task DeleteByAreaAsyc(int areaId);
}