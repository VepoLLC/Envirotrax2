using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Crm;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Crm;

public interface ICompanyContactService
{
    Task<IPagedData<CompanyContactDto>> GetAllAsync(int companyId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<CompanyContactDto?> AddAsync(int companyId, CompanyContactDto contact, CancellationToken cancellationToken);
    Task<CompanyContactDto?> UpdateAsync(int companyId, int contactId, CompanyContactDto contact, CancellationToken cancellationToken);
    Task DeleteAsync(int companyId, int contactId, CancellationToken cancellationToken);
}
