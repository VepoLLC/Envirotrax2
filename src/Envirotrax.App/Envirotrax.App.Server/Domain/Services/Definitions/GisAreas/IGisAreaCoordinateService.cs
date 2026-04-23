using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;

namespace Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;

public interface IGisAreaCoordinateService : IService<GisAreaCoordinate, GisAreaCoordinateDto, long>
{
    Task<IEnumerable<GisAreaCoordinateDto>> GetByAreaIdAsync(int areaId, CancellationToken cancellationToken);
    Task<IEnumerable<GisAreaCoordinateDto>> AddOrUpdateAsync(int areaId, IEnumerable<GisAreaCoordinateDto> coordinates);
    Task DeleteByAreaAsync(int areaId);
}
