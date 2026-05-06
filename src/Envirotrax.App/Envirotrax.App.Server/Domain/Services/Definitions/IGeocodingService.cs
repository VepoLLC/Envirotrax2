
using Envirotrax.App.Server.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.Services.Definitions;

public interface IGeocodingService
{
    Task<GeocodingResponseDto> GeocodeAsync(string address, CancellationToken cancellationToken);

    bool IsPointInArea(IList<CoordinateDto> points, CoordinateDto point);
}