using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

[Table("BackflowRenewalRequirements")]
public class BackflowRenewalRequirement : TenantModel<WaterSupplier>, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public PropertyType PropertyType { get; set; }

    [MaxLength(50)]
    public string? DeviceType { get; set; }

    [MaxLength(50)]
    public string? HazardType { get; set; }

    public bool HasSiteOssf { get; set; }

    public bool AuxWaterSupply { get; set; }

    public int RenewalYears { get; set; }

    // Audit
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}
