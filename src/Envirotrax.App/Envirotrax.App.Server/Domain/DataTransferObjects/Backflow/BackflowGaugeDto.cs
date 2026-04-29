
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowGaugeDto : IDto
{
    public int Id { get; set; }

    public ReferencedProfessionalDto? Professional { get; set; }

    [Required]
    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string SerialNumber { get; set; } = null!;

    public DateTime? LastCalibrationDate { get; set; }

    public bool IsPortable { get; set; }

    public bool IsManaged { get; set; }

    public string? FilePath { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }
}

public class CreateBackflowGaugeDto
{
    [Required]
    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string SerialNumber { get; set; } = null!;

    public DateTime? LastCalibrationDate { get; set; }

    public bool IsPortable { get; set; }

    public bool IsManaged { get; set; }

    public string? FilePath { get; set; }
}
