
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Envirotrax.LegacyDataMigration.Data;

namespace Envirotrax.LegacyDataMigration.Data.Users;

public class UserRole : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AspNetUserBase? User { get; set; }

    [AppPrimaryKey(false)]
    public int RoleId { get; set; }
    public Role? Role { get; set; }
}