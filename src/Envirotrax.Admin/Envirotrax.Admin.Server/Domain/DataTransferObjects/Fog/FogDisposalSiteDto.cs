using System.ComponentModel.DataAnnotations;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// Mirrors the App's FogDisposalSiteDto, validation included, so a bad save is rejected here before the proxy hop.
/// </summary>
public class FogDisposalSiteDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    public StateDto? State { get; set; }

    [StringLength(50)]
    public string? ZipCode { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(255)]
    public string? EmailAddress { get; set; }

    [Required]
    [StringLength(50)]
    public string County { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string TceqRegion { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string RegistrationNumber { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string PermitNumber { get; set; } = null!;

    public PhysicalType PhysicalType { get; set; }

    [StringLength(255)]
    public string? LocationDescription { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
