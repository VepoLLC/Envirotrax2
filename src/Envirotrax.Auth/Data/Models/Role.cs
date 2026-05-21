
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Auth.Data.Models;

[ExcludedModel]
[ReadOnlyModel]
public class Role : TenantModel<WaterSupplier>, IDeleteAutitableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}