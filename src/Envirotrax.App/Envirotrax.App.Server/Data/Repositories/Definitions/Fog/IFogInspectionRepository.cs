using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogInspectionRepository : IRepository<FogInspection>
{
    Task<IEnumerable<FogInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query,
        bool latestOnly, CancellationToken cancellationToken);

    Task<IEnumerable<FogInspection>> SearchForWaterSupplierAsync(
        PageInfo pageInfo, Query query,
        int? subAccountWaterSupplierId, CancellationToken cancellationToken);
}
