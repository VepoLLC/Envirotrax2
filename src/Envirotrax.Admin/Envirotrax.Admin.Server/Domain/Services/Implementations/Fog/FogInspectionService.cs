using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogInspectionService : IFogInspectionService
{
    private const string BaseUrl = "/api/admin/fog/inspections";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogInspectionService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<FogInspectionDto>> SearchAsync(
        PageInfo pageInfo, Query query,
        FogPaymentStatus? paymentStatus, FogTotalCapacityRange? totalCapacityRange,
        CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>();

        if (paymentStatus.HasValue)
        {
            additionalParameters["paymentStatus"] = ((int)paymentStatus.Value).ToString();
        }

        if (totalCapacityRange.HasValue)
        {
            additionalParameters["totalCapacityRange"] = ((int)totalCapacityRange.Value).ToString();
        }

        return _apiClient.GetAsync<FogInspectionDto>(BaseUrl, pageInfo, query, additionalParameters, cancellationToken);
    }
}
