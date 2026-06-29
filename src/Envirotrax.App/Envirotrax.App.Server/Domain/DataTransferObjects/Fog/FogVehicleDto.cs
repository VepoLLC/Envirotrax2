using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogVehicleDto : IDto
{
    public int Id { get; set; }

    public ReferencedProfessionalDto? Professional { get; set; }

    [Required]
    [MaxLength(20)]
    public string LicensePlateNumber { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Manufacturer { get; set; } = null!;

    public int ManufacturedYear { get; set; }

    public double Capacity { get; set; }

    public FogVehicleCapacityType CapacityType { get; set; }

    [Required]
    [MaxLength(50)]
    public string StickerNumber { get; set; } = null!;

    // Audit
    public DateTime CreatedTime { get; set; }
}
