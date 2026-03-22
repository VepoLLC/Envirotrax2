
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

    public bool IsAdmin { get; set; }

    public bool IsWiseGuy { get; set; }
    public bool IsCsiInspector { get; set; }
    public bool IsBackflowTester { get; set; }
    public bool IsFogInspector { get; set; }
    public bool IsFogTransporter { get; set; }
}