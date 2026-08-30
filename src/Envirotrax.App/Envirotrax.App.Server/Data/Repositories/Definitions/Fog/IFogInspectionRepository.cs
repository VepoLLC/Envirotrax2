using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogInspectionRepository : IRepository<FogInspection>
{
    Task<IEnumerable<FogInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query,
        bool latestOnly, CancellationToken cancellationToken);

    // Dashboard "View" on a sub account: shows that child water supplier's own inspections without
    // switching the current session's authentication.
    Task<IEnumerable<FogInspection>> SearchForSubAccountAsync(
        PageInfo pageInfo, Query query,
        int subAccountWaterSupplierId, CancellationToken cancellationToken);
}
