using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

// One row of the inspection's "Assemblies at this Location" list, as it stands when the inspector
// completes the submission. The row kind follows from which ids are set:
//   Id set             → a row already saved against the inspection
//   TestId set, no Id  → a current test at the site, not yet saved against the inspection
//   neither            → an assembly the inspector added, which becomes a new BackflowTest
public class CsiInspectionAssemblyRequest
{
    public int? Id { get; set; }

    public int? TestId { get; set; }

    public bool VisuallyIdentified { get; set; }

    // New assembly only. Manufacturer, model and size are kept short enough that the
    // "{manufacturer} {model} {size} - {device type}" description fits AssemblyDescription.
    [MaxLength(50)]
    public string? DeviceType { get; set; }

    [MaxLength(50)]
    public string? Manufacturer { get; set; }

    [MaxLength(50)]
    public string? Model { get; set; }

    [MaxLength(20)]
    public string? Size { get; set; }

    [MaxLength(50)]
    public string? SerialNumber { get; set; }

    [MaxLength(50)]
    public string? Manufacturer2 { get; set; }

    [MaxLength(50)]
    public string? Model2 { get; set; }

    [MaxLength(20)]
    public string? Size2 { get; set; }

    [MaxLength(50)]
    public string? SerialNumber2 { get; set; }

    [MaxLength(50)]
    public string? HazardType { get; set; }

    [MaxLength(200)]
    public string? HazardTypeOtherDescription { get; set; }

    [MaxLength(200)]
    public string? LocationDescription { get; set; }

    public string? Comments { get; set; }
}
