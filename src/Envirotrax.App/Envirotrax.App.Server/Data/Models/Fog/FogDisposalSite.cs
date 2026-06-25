using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Fog;

[Table("FogDisposalSites")]
public class FogDisposalSite : ICreateAuditableModel<AppUser>, IDeleteAutitableModel<AppUser>
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    public int? StateId { get; set; }
    public State? State { get; set; }

    [MaxLength(50)]
    public string? ZipCode { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(255)]
    public string? EmailAddress { get; set; }

    [Required]
    [MaxLength(50)]
    public string County { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string TceqRegion { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string RegistrationNumber { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string PermitNumber { get; set; } = null!;

    public PhysicalType PhysicalType { get; set; }

    [MaxLength(255)]
    public string? LocationDescription { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    // Audit
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}
