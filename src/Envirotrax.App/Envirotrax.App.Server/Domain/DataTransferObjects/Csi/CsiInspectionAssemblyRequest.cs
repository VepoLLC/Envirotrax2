using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

// An assembly the inspector adds on the inspection form ("+ Add Assembly"). It is saved straight away,
// under the form's SubmissionId, and linked to the inspection when the inspection is submitted.
public class CsiInspectionAssemblyRequest
{
    [Required]
    [MaxLength(50)]
    public string SubmissionId { get; set; } = null!;

    public int SiteId { get; set; }

    [Required]
    [MaxLength(50)]
    public string DeviceType { get; set; } = null!;

    [MaxLength(50)]
    public string? Manufacturer { get; set; }

    [MaxLength(50)]
    public string? Model { get; set; }

    [MaxLength(50)]
    public string? Size { get; set; }

    [MaxLength(50)]
    public string? SerialNumber { get; set; }

    [MaxLength(50)]
    public string? Manufacturer2 { get; set; }

    [MaxLength(50)]
    public string? Model2 { get; set; }

    [MaxLength(50)]
    public string? Size2 { get; set; }

    [MaxLength(50)]
    public string? SerialNumber2 { get; set; }

    [Required]
    [MaxLength(50)]
    public string HazardType { get; set; } = null!;

    [MaxLength(200)]
    public string? HazardTypeOtherDescription { get; set; }

    [MaxLength(200)]
    public string? LocationDescription { get; set; }

    public string? Comments { get; set; }
}
