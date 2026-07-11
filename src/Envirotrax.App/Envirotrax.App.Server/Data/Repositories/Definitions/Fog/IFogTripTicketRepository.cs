using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogTripTicketRepository : IRepository<FogTripTicket>
{
    Task<IEnumerable<FogTripTicket>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken ct);
}
