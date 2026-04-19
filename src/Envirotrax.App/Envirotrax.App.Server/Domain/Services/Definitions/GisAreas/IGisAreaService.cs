using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;

namespace Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;

public interface IGisAreaService : IService<GisArea, GisAreaDto>
{
    Task<DefaultGiisMapViewDto> GetDefaultMapViewAsync(CancellationToken cancellationToken);
    Task<DefaultGiisMapViewDto> UpdateDefaultMapViewAsync(DefaultGiisMapViewDto mapView);
}
