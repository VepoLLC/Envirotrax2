using System.ComponentModel.DataAnnotations;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

/// <summary>
/// Mirrors the App's BackflowGaugeDto. FilePath has to round-trip on edit: the App's gauge update copies it from
/// the request, so dropping it would detach the calibration certificate.
/// </summary>
public class BackflowGaugeDto
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

    public string? FilePath { get; set; }

    public ExpirationType ExpirationType { get; set; }

    public DateTime CreatedTime { get; set; }
}
