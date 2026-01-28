
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.Professionals;

public class ProfessionalUser
{
    [AppPrimaryKey(false)]
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    [Required]
    [StringLength(255)]
    public string ContactName { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string JobTitle { get; set; } = null!;

    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}