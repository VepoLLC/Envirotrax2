
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class WaterSupplierUser : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}