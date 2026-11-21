using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Crm;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Crm;

public class CompanyContactService : ICompanyContactService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public CompanyContactService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private static string GetContactsUrl(int companyId)
    {
        return $"/api/admin/crm/companies/{companyId}/contacts";
    }

    public Task<IPagedData<CompanyContactDto>> GetAllAsync(int companyId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<CompanyContactDto>(GetContactsUrl(companyId), pageInfo, query, cancellationToken);
    }

    // The App puts a contact under its company's water supplier, so the writes use the tenant-less overloads.
    public Task<CompanyContactDto?> AddAsync(int companyId, CompanyContactDto contact, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<CompanyContactDto, CompanyContactDto>(GetContactsUrl(companyId), contact, cancellationToken);
    }

    public Task<CompanyContactDto?> UpdateAsync(int companyId, int contactId, CompanyContactDto contact, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<CompanyContactDto, CompanyContactDto>($"{GetContactsUrl(companyId)}/{contactId}", contact, cancellationToken);
    }

    public Task DeleteAsync(int companyId, int contactId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{GetContactsUrl(companyId)}/{contactId}", cancellationToken);
    }
}
