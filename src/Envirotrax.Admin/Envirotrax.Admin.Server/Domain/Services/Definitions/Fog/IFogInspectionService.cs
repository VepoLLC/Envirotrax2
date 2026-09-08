using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

public interface IFogInspectionService
{
    Task<IPagedData<FogInspectionDto>> SearchAsync(
        PageInfo pageInfo, Query query,
        FogPaymentStatus? paymentStatus, FogTotalCapacityRange? totalCapacityRange,
        CancellationToken cancellationToken);
}
