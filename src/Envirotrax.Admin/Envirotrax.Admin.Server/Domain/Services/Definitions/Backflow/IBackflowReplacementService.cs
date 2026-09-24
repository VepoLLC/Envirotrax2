using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowReplacementService
{
    Task<IPagedData<BackflowReplacementDto>> GetAllAsync(PageInfo pageInfo, Query query, bool onHold, CancellationToken cancellationToken);

    Task<BackflowReplacementDto?> GetReplacedAssemblyAsync(int id, CancellationToken cancellationToken);

    Task<BackflowReplacementDto?> UpdateHoldAsync(int id, int waterSupplierId, bool onHold, CancellationToken cancellationToken);

    Task<BackflowReplacementDto?> UpdateClearedAsync(int id, int waterSupplierId, bool cleared, CancellationToken cancellationToken);
}
