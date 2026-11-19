using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Crm;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Crm;

public class CompanyService : ICompanyService
{
    private const string BaseUrl = "/api/admin/crm/companies";
    private const string UsersUrl = "/api/admin/crm/users";

    private readonly IEnvirotraxApiClient _apiClient;

    public CompanyService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<CompanyDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<CompanyDto>(BaseUrl, pageInfo, query, cancellationToken);
    }

    public Task<CompanyDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<CompanyDto>($"{BaseUrl}/{id}", cancellationToken);
    }

    // The admin sign-in carries no water supplier. The App puts the companies under the Vepo LLC supplier itself,
    // so the writes use the tenant-less overloads.
    public Task<CompanyDto?> AddAsync(CompanyDto company, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<CompanyDto, CompanyDto>(BaseUrl, company, cancellationToken);
    }

    public Task<CompanyDto?> UpdateAsync(int id, CompanyDto company, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<CompanyDto, CompanyDto>($"{BaseUrl}/{id}", company, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{id}", cancellationToken);
    }

    public Task<IPagedData<WaterSupplierUserDto>> GetCrmUsersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<WaterSupplierUserDto>(UsersUrl, pageInfo, query, cancellationToken);
    }
}
