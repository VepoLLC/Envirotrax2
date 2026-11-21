using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Crm;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Crm;

public interface ICompanyContactService : IService<CompanyContactDto>
{
    Task<IPagedData<CompanyContactDto>> GetAllByCompanyAsync(int companyId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    // Each returns null when the company, or the contact in that company, is not found.
    Task<CompanyContactDto?> AddForCompanyAsync(int companyId, CompanyContactDto contact, CancellationToken cancellationToken);
    Task<CompanyContactDto?> UpdateForCompanyAsync(int companyId, int contactId, CompanyContactDto contact, CancellationToken cancellationToken);
    Task<CompanyContactDto?> DeleteForCompanyAsync(int companyId, int contactId, CancellationToken cancellationToken);
}
