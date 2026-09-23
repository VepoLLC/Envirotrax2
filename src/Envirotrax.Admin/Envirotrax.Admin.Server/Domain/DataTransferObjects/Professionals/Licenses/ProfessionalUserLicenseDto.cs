using System.ComponentModel.DataAnnotations;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class ProfessionalUserLicenseDto
{
    public int Id { get; set; }

    [Required]
    public ReferencedProfessionalUserDto User { get; set; } = null!;

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

public enum ProfessionalType
{
    Contractor,
    PlanChecker,
    Bpat,
    Inspector,
    CsiInspector,
    FogTransporter,
    FogInspector,
    ComponentTester,
}
