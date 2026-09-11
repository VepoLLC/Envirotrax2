
using DeveloperPartners.SortingFiltering;
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

    public Task<IPagedData<ProfessionalDto>> SearchAsync(PageInfo pageInfo, Query query, IDictionary<string, string> criteria, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalDto>(BaseUrl, pageInfo, query, criteria, cancellationToken);
    }
}
