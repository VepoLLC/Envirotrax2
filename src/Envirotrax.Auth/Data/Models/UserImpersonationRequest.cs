
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Auth.Data.Models;

[ExcludedModel]
public class UserImpersonationRequest : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int SourceUserId { get; set; }
    public AppUser? SourceUser { get; set; }

    public int TargetUserId { get; set; }
    public AppUser? TargetUser { get; set; }

    public DateTime CreatedTime { get; set; }
}
