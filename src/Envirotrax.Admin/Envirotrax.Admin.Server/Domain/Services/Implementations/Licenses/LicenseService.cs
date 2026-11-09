using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Licenses;

public class LicenseService : ILicenseService
{
    private const string BaseUrl = "/api/admin/licenses";
    private const string CompanyLicensesUrl = "/api/admin/company-licenses";

    private readonly IEnvirotraxApiClient _apiClient;

    public LicenseService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<WaterSupplierLicenseDto>> GetAllAsync(string? licenseFilter, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>();

        if (licenseFilter != null)
        {
            additionalParameters["licenseFilter"] = licenseFilter;
        }

        return _apiClient.GetAsync<WaterSupplierLicenseDto>(BaseUrl, pageInfo, query, additionalParameters, cancellationToken);
    }

    public Task<LicenseAdminCountsDto?> GetCountsAsync(CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<LicenseAdminCountsDto>($"{BaseUrl}/counts", cancellationToken);
    }

    public Task<List<ProfessionalLicenseTypeDto>?> GetTypesAsync(CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<ProfessionalLicenseTypeDto>>($"{BaseUrl}/types", cancellationToken);
    }

    public Task<ProfessionalUserLicenseDto?> UpdateAsync(int id, ProfessionalUserLicenseDto license)
    {
        return _apiClient.PutAsync<ProfessionalUserLicenseDto, ProfessionalUserLicenseDto>($"{BaseUrl}/{id}", license, CancellationToken.None);
    }

    public Task<TceqLicenseDto?> CheckAsync(int id)
    {
        return _apiClient.PostAsync<object?, TceqLicenseDto>($"{BaseUrl}/{id}/checks", null, CancellationToken.None);
    }

    public Task<ProfessionalLicenseDto?> UpdateCompanyLicenseAsync(int id, ProfessionalLicenseDto license)
    {
        return _apiClient.PutAsync<ProfessionalLicenseDto, ProfessionalLicenseDto>($"{CompanyLicensesUrl}/{id}", license, CancellationToken.None);
    }
}
