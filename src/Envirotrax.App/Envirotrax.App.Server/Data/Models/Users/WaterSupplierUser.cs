
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Users;

public class WaterSupplierUser : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }

    //[Required]
    //[StringLength(100)]
    //public string ContactName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string EmailAddress { get; set; } = null!;
}