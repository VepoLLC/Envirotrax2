
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalDashboardService
{
    Task<IPagedData<ProfessionalDashboardLicenseInsuranceDto>> GetLicensesAndInsurancesAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
