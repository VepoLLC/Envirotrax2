using System.ComponentModel.DataAnnotations;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

/// <summary>
/// Mirrors the App's ProfessionalLicenseDto: a company-level license (ProfessionalLicenses), which has no user,
/// unlike ProfessionalUserLicenseDto. FOG transporter registrations are this kind.
/// </summary>
public class ProfessionalLicenseDto
{
    public int Id { get; set; }

    [Required]
    public ProfessionalType? ProfessionalType { get; set; }

    [Required]
    public ReferencedProfessionalLicenseTypeDto LicenseType { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LicenseNumber { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    public ExpirationType ExpirationType { get; set; }
}
