using Envirotrax.App.Server.Data.Models.GisAreas;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;

public interface IGisAreaRepository : IRepository<GisArea>
{
    Task<DefaultGisMapView> GetDefaultMapViewAsync(CancellationToken cancellationToken);
    Task<DefaultGisMapView> UpdateDefaultMapViewAsync(DefaultGisMapView mapView);
}