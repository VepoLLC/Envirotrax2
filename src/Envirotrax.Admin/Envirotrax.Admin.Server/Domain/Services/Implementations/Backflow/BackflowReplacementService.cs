using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Backflow;

public class BackflowReplacementService : IBackflowReplacementService
{
    private const string BaseUrl = "/api/admin/backflow/replacements";

    private readonly IEnvirotraxApiClient _apiClient;

    public BackflowReplacementService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<BackflowReplacementDto>> GetAllAsync(PageInfo pageInfo, Query query, bool onHold, CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>
        {
            ["onHold"] = onHold ? "true" : "false"
        };

        return _apiClient.GetAsync<BackflowReplacementDto>(BaseUrl, pageInfo, query, additionalParameters, cancellationToken);
    }

    public Task<BackflowReplacementDto?> GetReplacedAssemblyAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<BackflowReplacementDto>($"{BaseUrl}/{id}/replaced-assembly", cancellationToken);
    }

    public Task<BackflowReplacementDto?> UpdateHoldAsync(int id, int waterSupplierId, bool onHold, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<bool, BackflowReplacementDto>(waterSupplierId, $"{BaseUrl}/{id}/hold", onHold, cancellationToken);
    }

    public Task<BackflowReplacementDto?> UpdateClearedAsync(int id, int waterSupplierId, bool cleared, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<bool, BackflowReplacementDto>(waterSupplierId, $"{BaseUrl}/{id}/cleared", cleared, cancellationToken);
    }
}
