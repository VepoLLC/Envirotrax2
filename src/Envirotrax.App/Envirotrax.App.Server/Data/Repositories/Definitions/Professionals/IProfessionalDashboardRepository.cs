
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalDashboardRepository
{
    Task<IEnumerable<ProfessionalDashboardLicenseInsuranceDto>> GetLicensesAndInsurancesAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
