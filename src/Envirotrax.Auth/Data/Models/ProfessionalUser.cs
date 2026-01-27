
using System.Diagnostics.Contracts;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.Auth.Data.Models;

[ExcludedModel]
[ReadOnlyModel]
public class ProfessionalUser
{
    [AppPrimaryKey(false)]
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}