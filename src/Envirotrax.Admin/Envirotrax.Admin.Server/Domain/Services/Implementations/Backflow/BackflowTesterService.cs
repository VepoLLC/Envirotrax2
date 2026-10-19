
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Backflow;

public class BackflowTesterService : IBackflowTesterService
{
    private const string BaseUrl = "/api/admin/backflow/testers";

    private readonly IEnvirotraxApiClient _apiClient;

    public BackflowTesterService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<ProfessionalDto>> SearchAsync(BackflowTesterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalDto>(BaseUrl, pageInfo, query, criteria.ToParameters(), cancellationToken);
    }

    public Task<ProfessionalDto?> GetAsync(int professionalId, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalDto>($"{BaseUrl}/{professionalId}", cancellationToken);
    }

    // Professionals are not owned by a water supplier, so this uses the tenant-less write overload.
    public Task<ProfessionalDto?> UpdateAsync(int professionalId, ProfessionalDto professional, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<ProfessionalDto, ProfessionalDto>($"{BaseUrl}/{professionalId}", professional, cancellationToken);
    }
}
