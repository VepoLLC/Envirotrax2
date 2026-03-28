
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;

public interface IProfessionalLicenseTypeRepository
{
    Task<IEnumerable<ProfessionalLicenseType>> GetAllAsync(Query query, CancellationToken cancellationToken);
}