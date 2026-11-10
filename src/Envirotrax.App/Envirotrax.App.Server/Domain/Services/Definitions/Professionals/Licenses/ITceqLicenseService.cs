using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

public interface ITceqLicenseService
{
    Task<TceqLicenseDto?> FindBpatLicenseAsync(string licenseNumber, CancellationToken cancellationToken);
}
