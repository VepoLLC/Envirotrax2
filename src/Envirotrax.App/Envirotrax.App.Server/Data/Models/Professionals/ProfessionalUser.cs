
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Envirotrax.App.Server.Data.Models.States;
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

    [StringLength(500)]
    public string? SignaturePath { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsWiseGuy { get; set; }
    public bool IsCsiInspector { get; set; }
    public bool IsBackflowTester { get; set; }
    public bool IsFogInspector { get; set; }
    public bool IsFogTransporter { get; set; }

    [StringLength(255)]
    public string? BillingFirstName { get; set; }

    [StringLength(255)]
    public string? BillingLastName { get; set; }

    [StringLength(255)]
    public string? BillingAddress { get; set; }

    [StringLength(255)]
    public string? BillingCity { get; set; }

    public int? BillingStateId { get; set; }
    public State? BillingState { get; set; }

    [StringLength(25)]
    public string? BillingZipCode { get; set; }
}