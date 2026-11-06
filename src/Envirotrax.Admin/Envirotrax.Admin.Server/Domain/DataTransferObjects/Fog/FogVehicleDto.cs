using System.ComponentModel.DataAnnotations;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// Mirrors the App's FogVehicleDto, validation included, so a bad save is rejected here before the proxy hop.
/// </summary>
public class FogVehicleDto
{
    public int Id { get; set; }

    public ReferencedProfessionalDto? Professional { get; set; }

    [Required]
    [StringLength(20)]
    public string LicensePlateNumber { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string Manufacturer { get; set; } = null!;

    public int ManufacturedYear { get; set; }

    public double Capacity { get; set; }

    public FogVehicleCapacityType CapacityType { get; set; }

    [Required]
    [StringLength(50)]
    public string StickerNumber { get; set; } = null!;

    public DateTime CreatedTime { get; set; }
}
