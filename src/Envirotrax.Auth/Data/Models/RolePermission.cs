
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Auth.Data.Models;

[ReadOnlyModel]
[ExcludedModel]
public class RolePermission : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int RoleId { get; set; }

    [AppPrimaryKey(false)]
    public int PermissionId { get; set; }

    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}