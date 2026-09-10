
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalDashboardService : IProfessionalDashboardService
{
    private readonly IProfessionalDashboardRepository _dashboardRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public ProfessionalDashboardService(
        IProfessionalDashboardRepository dashboardRepository,
        ITimeZoneHelperService timeZoneHelper)
    {
        _dashboardRepository = dashboardRepository;
        _timeZoneHelper = timeZoneHelper;
    }

    public async Task<IPagedData<ProfessionalDashboardLicenseInsuranceDto>> GetLicensesAndInsurancesAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var rows = await _dashboardRepository.GetLicensesAndInsurancesAsync(pageInfo, query, cancellationToken);

        // Expiration is relative to the viewer's clock, so it is computed here rather than stored or queried.
        var now = _timeZoneHelper.GetUserLocalTime();

        foreach (var row in rows)
        {
            row.ExpirationType = row.ExpirationDate.HasValue
                ? (row.ExpirationDate < now ? ExpirationType.Expired
                    : row.ExpirationDate < now.AddDays(30) ? ExpirationType.AboutToExpire
                    : ExpirationType.Valid)
                : ExpirationType.Valid;
        }

        return rows.ToPagedData(pageInfo);
    }
}
