using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class WaterSupplierLicenseDto : IDto
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? CompanyName { get; set; }
    public string? ContactName { get; set; }
    public ProfessionalType ProfessionalType { get; set; }
    public int LicenseTypeId { get; set; }
    public string? LicenseTypeName { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public DateTime? ExpirationDate { get; set; }
    public ExpirationType ExpirationType { get; set; }
}
