
using System.Diagnostics.Contracts;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.Professionals;

public class ProfessionalUser
{
    [AppPrimaryKey(false)]
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}