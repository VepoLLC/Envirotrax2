
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Professionals;

public class Professional : IAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }
    public Professional? Parent { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    public int? StateId { get; set; }
    public State? State { get; set; }

    [StringLength(255)]
    public string? City { get; set; }

    [StringLength(25)]
    public string? ZipCode { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    public string? FaxNumber { get; set; }

    [StringLength(255)]
    public string? WebSiteUrl { get; set; }

    public bool HidePublicListing { get; set; }

    public bool HasWiseGuys { get; set; }
    public bool HasBackflowTesting { get; set; }
    public bool HasCsiInspection { get; set; }
    public bool HasFogInspection { get; set; }
    public bool HasFogTransportation { get; set; }

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? UpdatedById { get; set; }
    public AppUser? UpdatedBy { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}