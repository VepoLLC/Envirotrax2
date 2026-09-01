using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Csi;

public class CsiInspectorUserService : ICsiInspectorUserService
{
    private const string BaseUrl = "/api/admin/csi/inspectors";

    private readonly IEnvirotraxApiClient _apiClient;

    public CsiInspectorUserService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<ProfessionalUserDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalUserDto>($"{BaseUrl}/{professionalId}/users", pageInfo, query, cancellationToken);
    }

    public Task<ProfessionalUserDto?> AddAsync(int professionalId, ProfessionalUserDto user, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<ProfessionalUserDto, ProfessionalUserDto>($"{BaseUrl}/{professionalId}/users", user, cancellationToken);
    }

    public Task<ProfessionalUserDto?> UpdateAsync(int professionalId, int userId, ProfessionalUserDto user, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<ProfessionalUserDto, ProfessionalUserDto>($"{BaseUrl}/{professionalId}/users/{userId}", user, cancellationToken);
    }

    public Task DeleteAsync(int professionalId, int userId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{professionalId}/users/{userId}", cancellationToken);
    }
}
