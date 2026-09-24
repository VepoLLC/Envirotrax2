

using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;

public class GisAreaCoordinateDto
{
    public long Id { get; set; }

    [Required]
    public ReferencedGisAreaDto Area { get; set; } = null!;

    public int PolygonIndex { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}