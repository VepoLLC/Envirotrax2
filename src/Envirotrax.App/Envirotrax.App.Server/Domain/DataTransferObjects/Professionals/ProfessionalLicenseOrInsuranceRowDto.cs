
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalLicenseOrInsuranceRowDto
{
    public string LicenseType { get; set; } = null!;

    public string Number { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    public ExpirationType ExpirationType { get; set; }
}
