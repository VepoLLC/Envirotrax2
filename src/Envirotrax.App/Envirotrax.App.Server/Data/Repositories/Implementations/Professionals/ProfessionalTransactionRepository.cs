using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalTransactionRepository : Repository<ProfessionalTransaction>, IProfessionalTransactionRepository
{
    public ProfessionalTransactionRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<ProfessionalTransaction?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken)
    {
        return await Entity
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleOrDefaultAsync(transaction => transaction.TransactionId == transactionId, cancellationToken);
    }
}
