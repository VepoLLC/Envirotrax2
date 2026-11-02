using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogTransporterLicenseService : IFogTransporterLicenseService
{
    private const string BaseUrl = "/api/admin/fog/transporters";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogTransporterLicenseService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<List<ProfessionalLicenseTypeDto>?> GetTypesAsync(CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<ProfessionalLicenseTypeDto>>($"{BaseUrl}/licenses/types", cancellationToken);
    }

    public Task<IPagedData<ProfessionalLicenseDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalLicenseDto>($"{BaseUrl}/{professionalId}/licenses", pageInfo, query, cancellationToken);
    }

    public Task<ProfessionalLicenseDto?> AddAsync(int professionalId, ProfessionalLicenseDto license, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<ProfessionalLicenseDto, ProfessionalLicenseDto>($"{BaseUrl}/{professionalId}/licenses", license, cancellationToken);
    }

    public Task<ProfessionalLicenseDto?> UpdateAsync(int professionalId, int licenseId, ProfessionalLicenseDto license, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<ProfessionalLicenseDto, ProfessionalLicenseDto>($"{BaseUrl}/{professionalId}/licenses/{licenseId}", license, cancellationToken);
    }

    public Task DeleteAsync(int professionalId, int licenseId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{professionalId}/licenses/{licenseId}", cancellationToken);
    }
}
