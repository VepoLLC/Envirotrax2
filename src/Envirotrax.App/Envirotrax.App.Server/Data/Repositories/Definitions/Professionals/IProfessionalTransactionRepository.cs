using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalTransactionRepository : IRepository<ProfessionalTransaction>
{
    Task<ProfessionalTransaction?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken);
}
