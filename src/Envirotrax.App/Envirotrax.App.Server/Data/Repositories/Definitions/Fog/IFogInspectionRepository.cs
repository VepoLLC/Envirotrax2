using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogInspectionRepository : IRepository<FogInspection>
{
    Task<IEnumerable<FogInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query,
        bool latestOnly, CancellationToken cancellationToken);

    Task<IEnumerable<FogInspection>> SearchForAdminAsync(
        PageInfo pageInfo, Query query,
        FogPaymentStatus? paymentStatus, FogTotalCapacityRange? totalCapacityRange,
        CancellationToken cancellationToken);
}
