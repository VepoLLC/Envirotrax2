
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
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

    public Task<IPagedData<BackflowTesterAccountDto>> SearchAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(licenseNumber))
        {
            additionalParameters["licenseNumber"] = licenseNumber;
        }

        if (!string.IsNullOrWhiteSpace(insuranceNumber))
        {
            additionalParameters["insuranceNumber"] = insuranceNumber;
        }

        return _apiClient.GetAsync<BackflowTesterAccountDto>(BaseUrl, pageInfo, query, additionalParameters, cancellationToken);
    }
}
