
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

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

    public string? SignaturePath { get; set; }

    public string? SignatureUrl { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsWiseGuy { get; set; }
    public bool IsCsiInspector { get; set; }
    public bool IsBackflowTester { get; set; }
    public bool IsFogInspector { get; set; }
    public bool IsFogTransporter { get; set; }

    public string? BpatLicenseNumber { get; set; }
    public string? BpatLicenseTypeName { get; set; }
    public DateTime? BpatLicenseExpirationDate { get; set; }
    public ExpirationType? BpatLicenseExpirationType { get; set; }

    [StringLength(255)]
    public string? BillingFirstName { get; set; }

    [StringLength(255)]
    public string? BillingLastName { get; set; }

    [StringLength(255)]
    public string? BillingAddress { get; set; }

    [StringLength(255)]
    public string? BillingCity { get; set; }

    public ReferencedStateDto? BillingState { get; set; }

    [StringLength(25)]
    public string? BillingZipCode { get; set; }
}

public class ReferencedProfessionalUserDto
{
    [Required]
    public int? Id { get; set; }

    public string? EmailAddress { get; set; }

    public string? ContactName { get; set; }
}