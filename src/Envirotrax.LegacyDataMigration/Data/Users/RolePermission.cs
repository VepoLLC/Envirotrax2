
using Envirotrax.Common;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Envirotrax.LegacyDataMigration.Data;

namespace Envirotrax.LegacyDataMigration.Data.Users;

public class RolePermission : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int RoleId { get; set; }
    public Role? Role { get; set; }

    [AppPrimaryKey(false)]
    public PermissionType PermissionId { get; set; }

    public bool CanView { get; set; }
    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
}