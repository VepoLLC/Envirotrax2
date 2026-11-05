namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class TceqLicenseDto
{
    public string Name { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
}
