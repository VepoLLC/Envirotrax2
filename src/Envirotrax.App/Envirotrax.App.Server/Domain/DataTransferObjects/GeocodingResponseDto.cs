
namespace Envirotrax.App.Server.Domain.DataTransferObjects;

public class GeocodingResponseDto : CoordinateDto
{
}

public class CoordinateDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}