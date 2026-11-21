using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Crm;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Crm;

public interface ICompanyContactRepository : IRepository<CompanyContact>
{
    Task<IEnumerable<CompanyContact>> GetAllByCompanyAsync(int companyId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<bool> ExistsForCompanyAsync(int companyId, int contactId, CancellationToken cancellationToken);

    // Returns null when the contact is not one of the company's.
    Task<CompanyContact?> UpdateForCompanyAsync(int companyId, CompanyContact model, CancellationToken cancellationToken);
}
