
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.GisAreas;

public class GisArea : TenantModel<WaterSupplier>, IDeleteAutitableModel<AppUser>, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(10)]
    public string Color { get; set; } = null!;

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}

public class DefaultGisMapView
{
    public double? GisCenterLatitude { get; set; }
    public double? GisCenterLongitude { get; set; }
    public double? GisCenterZoom { get; set; }
}