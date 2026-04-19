
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;

public class GisAreaDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(10)]
    public string Color { get; set; } = null!;
}

public class ReferencedGisAreaDto
{
    [Required]
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Color { get; set; }
}

public class DefaultGiisMapViewDto
{
    [Required]
    public double? GisCenterLatitude { get; set; }

    [Required]
    public double? GisCenterLongitude { get; set; }

    [Required]
    public double? GisCenterZoom { get; set; }
}