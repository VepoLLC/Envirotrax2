
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Users;

public class RolePermission : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int RoleId { get; set; }
    public Role? Role { get; set; }

    [AppPrimaryKey(false)]
    public PermissionType PermissionId { get; set; }
    public Permission? Permission { get; set; }

    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}