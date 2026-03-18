
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Users;

public class UserRole : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }

    [AppPrimaryKey(false)]
    public int RoleId { get; set; }
    public Role? Role { get; set; }
}