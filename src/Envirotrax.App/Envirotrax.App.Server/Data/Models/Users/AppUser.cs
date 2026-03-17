
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Users;

[ReadOnlyModel]
[ExcludedModel]
public class AppUser : AspNetUserBase
{
    public string? Email { get; set; }
}