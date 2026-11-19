using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Crm;

public interface ICompanyService
{
    Task<IPagedData<CompanyDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<CompanyDto?> GetAsync(int id, CancellationToken cancellationToken);
    Task<CompanyDto?> AddAsync(CompanyDto company, CancellationToken cancellationToken);
    Task<CompanyDto?> UpdateAsync(int id, CompanyDto company, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IPagedData<WaterSupplierUserDto>> GetCrmUsersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
