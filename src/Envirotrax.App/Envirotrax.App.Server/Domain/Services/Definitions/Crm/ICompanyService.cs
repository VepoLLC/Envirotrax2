using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Crm;

public interface ICompanyService : IService<CompanyDto>
{
    /// <summary>
    /// The users of the water supplier that owns the CRM records - the ones a company's Last Contacted By can name.
    /// </summary>
    Task<IPagedData<WaterSupplierUserDto>> GetCrmUsersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
