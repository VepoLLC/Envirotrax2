
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Auth.Data.Models;

[ExcludedModel]
[ReadOnlyModel]
public class WaterSupplierUser : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}