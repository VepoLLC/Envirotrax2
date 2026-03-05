
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Auth.Data.Models;

[ExcludedModel]
[ReadOnlyModel]
public class UserRole : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int UserId { get; set; }

    [AppPrimaryKey(false)]
    public int RoleId { get; set; }
}