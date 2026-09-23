
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalRepository : IRepository<Professional>
{
    Task<IEnumerable<Professional>> GetAllMyAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<IEnumerable<Professional>> GetSubAccountsAsync(CancellationToken cancellationToken);

    Task<IAsyncDisposable?> TryAcquireBalanceLockAsync(int professionalId, CancellationToken cancellationToken);

    Task<bool> TryDebitBalanceAsync(int professionalId, decimal amount, CancellationToken cancellationToken);

    Task CreditBalanceAsync(int professionalId, decimal amount, CancellationToken cancellationToken);
}