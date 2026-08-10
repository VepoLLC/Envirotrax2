
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Lookup;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Lookup;

public class LookupService : ILookupService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public LookupService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<StateDto>> GetStatesAsync(CancellationToken cancellationToken)
    {
        var states = await _apiClient.GetAsync<List<StateDto>>("/api/admin/lookup/states", cancellationToken);
        return states ?? [];
    }
}
