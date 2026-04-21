

using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;

public class GisAreaCoordinateDto
{
    public long Id { get; set; }

    [Required]
    public ReferencedGisAreaDto Area { get; set; } = null!;

    public double Latitude { get; set; }
    public double Longitde { get; set; }
}