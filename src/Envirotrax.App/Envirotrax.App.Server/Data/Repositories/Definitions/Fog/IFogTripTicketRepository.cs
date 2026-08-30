using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogTripTicketRepository : IRepository<FogTripTicket>
{
    Task<IEnumerable<FogTripTicket>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken ct);

    // Dashboard "View" on a sub account: shows that child water supplier's own trip tickets without
    // switching the current session's authentication.
    Task<IEnumerable<FogTripTicket>> SearchForSubAccountAsync(PageInfo pageInfo, Query query, int subAccountWaterSupplierId, CancellationToken cancellationToken);

    Task<FogTripTicket?> UpdateApprovalAsync(int id, bool disapproved, int? approvedById, CancellationToken cancellationToken);
}
