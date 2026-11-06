namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class WaterSupplierLicenseDto
{
    public int Id { get; set; }

    public LicenseScope LicenseScope { get; set; }

    public int ProfessionalId { get; set; }

    public int? UserId { get; set; }

    public DateTime SubmittedOn { get; set; }

    public string? UserEmail { get; set; }

    public string? CompanyName { get; set; }

    public string? ContactName { get; set; }

    public ProfessionalType ProfessionalType { get; set; }

    public int LicenseTypeId { get; set; }

    public string? LicenseTypeName { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    public ExpirationType ExpirationType { get; set; }

    public DateTime? LastCheckDate { get; set; }

    public int? HoursSinceLastCheck { get; set; }
}

public enum LicenseScope
{
    User,
    Company
}
