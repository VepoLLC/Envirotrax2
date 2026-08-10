using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Backflow;

public class BackflowTestService : IBackflowTestService
{
    private const string BaseUrl = "/api/admin/backflow/tests";

    private readonly IEnvirotraxApiClient _apiClient;

    public BackflowTestService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<BackflowTestDto>> SearchAsync(PageInfo pageInfo, Query query, BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>();

        if (paymentStatus.HasValue)
        {
            additionalParameters["paymentStatus"] = ((int)paymentStatus.Value).ToString();
        }

        return _apiClient.GetAsync<BackflowTestDto>(BaseUrl, pageInfo, query, additionalParameters, cancellationToken);
    }
}
