
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalUserDto : IDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string EmailAddress { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string? ContactName { get; set; }

    [StringLength(150)]
    public string? JobTitle { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsWiseGuy { get; set; }
    public bool IsCsiInspector { get; set; }
    public bool IsBackflowTester { get; set; }
    public bool IsFogInspector { get; set; }
    public bool IsFogTransporter { get; set; }
}

public class ReferencedProfessionalUserDto
{
    [Required]
    public int? Id { get; set; }

    public string? EmailAddress { get; set; }

    public string? ContactName { get; set; }
}