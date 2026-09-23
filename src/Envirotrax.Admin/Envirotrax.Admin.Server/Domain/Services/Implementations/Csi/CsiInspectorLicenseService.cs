using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Csi;

public class CsiInspectorLicenseService : ICsiInspectorLicenseService
{
    private const string BaseUrl = "/api/admin/csi/inspectors";

    private readonly IEnvirotraxApiClient _apiClient;

    public CsiInspectorLicenseService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<List<ProfessionalLicenseTypeDto>?> GetTypesAsync(CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<ProfessionalLicenseTypeDto>>($"{BaseUrl}/licenses/types", cancellationToken);
    }

    public Task<IPagedData<ProfessionalUserLicenseDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalUserLicenseDto>($"{BaseUrl}/{professionalId}/licenses", pageInfo, query, cancellationToken);
    }

    public Task<ProfessionalUserLicenseDto?> AddAsync(int professionalId, ProfessionalUserLicenseDto license, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<ProfessionalUserLicenseDto, ProfessionalUserLicenseDto>($"{BaseUrl}/{professionalId}/licenses", license, cancellationToken);
    }

    public Task<ProfessionalUserLicenseDto?> UpdateAsync(int professionalId, int licenseId, ProfessionalUserLicenseDto license, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<ProfessionalUserLicenseDto, ProfessionalUserLicenseDto>($"{BaseUrl}/{professionalId}/licenses/{licenseId}", license, cancellationToken);
    }

    public Task DeleteAsync(int professionalId, int licenseId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{professionalId}/licenses/{licenseId}", cancellationToken);
    }
}
