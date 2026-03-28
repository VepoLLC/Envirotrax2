
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

public interface IProfessionalLicenseTypeService
{
    Task<IEnumerable<ProfessionalLicenseTypeDto>> GetAllAsync(Query query, CancellationToken cancellationToken);
}