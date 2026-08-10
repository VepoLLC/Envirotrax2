using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Fog;

[Table("FogVehicles")]
public class FogVehicle : IProfessionalModel, ICreateAuditableModel<AppUser>, IDeleteAutitableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

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
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }

    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}

public class FogVehiclePermitSearchResult
{
    public int VehicleId { get; set; }

    public int TransporterId { get; set; }
    public string TransporterCompanyName { get; set; } = null!;
    public string? TransporterAddress { get; set; }
    public string? TransporterCity { get; set; }
    public string? TransporterState { get; set; }
    public string? TransporterZip { get; set; }
    public string? TransporterPhoneNumber { get; set; }
    public string? TransporterFaxNumber { get; set; }
    public string? TransporterEmailAddress { get; set; }

    public string LicensePlateNumber { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public int ManufacturedYear { get; set; }
    public double Capacity { get; set; }
    public FogVehicleCapacityType CapacityType { get; set; }
    public string StickerNumber { get; set; } = null!;

    public bool HasPermit { get; set; }
    public string? PermitNumber { get; set; }
    public DateTime? InspectionDueDate { get; set; }
    public bool? IsActive { get; set; }
}
