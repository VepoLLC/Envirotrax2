using Envirotrax.App.Server.Data.Models.Crm;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Crm;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> UpdateCompanyAsync(Company model, CancellationToken cancellationToken);
}
