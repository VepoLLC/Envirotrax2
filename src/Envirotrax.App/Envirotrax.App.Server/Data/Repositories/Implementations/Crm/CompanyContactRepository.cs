using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Crm;
using Envirotrax.App.Server.Data.Repositories.Definitions.Crm;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Crm;

public class CompanyContactRepository : Repository<CompanyContact>, ICompanyContactRepository
{
    public CompanyContactRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<CompanyContact>> GetAllByCompanyAsync(int companyId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        // V1 lists a company's contacts in the order they were added.
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(CompanyContact.Id)] = SortOperator.Asc;
        }

        var paginated = await GetListQuery()
            .Where(contact => contact.CompanyId == companyId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForCompanyAsync(int companyId, int contactId, CancellationToken cancellationToken)
    {
        return Entity.AnyAsync(contact => contact.Id == contactId && contact.CompanyId == companyId, cancellationToken);
    }

    // Copies the editable fields onto the stored row, so the row keeps its company, its water supplier (part of
    // the key) and its creation audit. The admin context does not stamp a tenant on save.
    public async Task<CompanyContact?> UpdateForCompanyAsync(int companyId, CompanyContact model, CancellationToken cancellationToken)
    {
        var contact = await Entity.SingleOrDefaultAsync(contact => contact.Id == model.Id && contact.CompanyId == companyId, cancellationToken);

        if (contact == null)
        {
            return null;
        }

        contact.Position = model.Position;
        contact.ContactName = model.ContactName;
        contact.Address = model.Address;
        contact.City = model.City;
        contact.StateId = model.StateId;
        contact.ZipCode = model.ZipCode;
        contact.PhoneNumber = model.PhoneNumber;
        contact.CellNumber = model.CellNumber;
        contact.EmailAddress = model.EmailAddress;

        await SaveChangesAsync();

        return contact;
    }
}
