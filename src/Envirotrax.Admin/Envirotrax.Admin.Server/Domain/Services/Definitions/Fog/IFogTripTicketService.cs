using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

public interface IFogTripTicketService
{
    Task<IPagedData<FogTripTicketDto>> SearchAsync(FogTripTicketSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<FogTripTicketDto?> UpdateValidationHoldAsync(int id, int waterSupplierId, bool onHold, CancellationToken cancellationToken);

    Task<FogTripTicketDto?> UpdateNeedsValidationAsync(int id, int waterSupplierId, bool needsValidation, CancellationToken cancellationToken);
}
