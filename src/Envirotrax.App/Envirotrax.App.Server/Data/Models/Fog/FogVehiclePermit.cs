using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Fog;

[Table("FogVehiclePermits")]
public class FogVehiclePermit : TenantModel<WaterSupplier>, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(false)]
    public int VehicleId { get; set; }
    public FogVehicle? Vehicle { get; set; }

    [Required]
    [MaxLength(50)]
    public string PermitNumber { get; set; } = null!;

    public DateTime? InspectionDueDate { get; set; }

    public bool IsActive { get; set; }

    // Audit
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}
