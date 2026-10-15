namespace Envirotrax.App.Server.Data.Models.Professionals.Licenses;

public interface ILicenseModel
{
    string LicenseNumber { get; }

    DateTime? ExpirationDate { get; }

    ProfessionalLicenseType? LicenseType { get; }
}
