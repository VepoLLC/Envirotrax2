using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogTripTicketRepository : IRepository<FogTripTicket>
{
    Task<IEnumerable<FogTripTicket>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken ct);

    Task<FogTripTicket?> UpdateApprovalAsync(int id, bool disapproved, int? approvedById, CancellationToken cancellationToken);

    Task<int> CountBySiteAsync(int siteId, CancellationToken cancellationToken);

    // Checkout
    Task<List<FogTripTicket>> GetUnpaidForCheckoutAsync(IReadOnlyCollection<int> ids, int professionalId, int? transporterId, CancellationToken cancellationToken);
    Task<int> MarkPaidAsync(IReadOnlyCollection<int> ids, int professionalId, int? transporterId, string transactionId, DateTime transactionDate, IReadOnlyCollection<int> emailPdfTicketIds, CancellationToken cancellationToken);
    Task<decimal> SumAmountByTransactionIdAsync(string transactionId, int professionalId, CancellationToken cancellationToken);
    Task<List<FogTripTicket>> GetByTransactionIdAsync(string transactionId, int professionalId, CancellationToken cancellationToken);
}
