
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.Professionals;

public class ProfessionalUser : IProfessionalModel
{
    [AppPrimaryKey(false, IsShadowKey = true)]
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }

    [StringLength(255)]
    public string? ContactName { get; set; }

    [StringLength(150)]
    public string? JobTitle { get; set; }
}