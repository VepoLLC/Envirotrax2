
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionService : ICsiInspectionService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public CsiInspectionService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<CsiInspectionDto>> SearchAsync(PageInfo pageInfo, Query query, CsiPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>();

        if (paymentStatus.HasValue)
        {
            additionalParameters["paymentStatus"] = ((int)paymentStatus.Value).ToString();
        }

        return _apiClient.GetAsync<CsiInspectionDto>("/api/admin/csi/inspections", pageInfo, query, additionalParameters, cancellationToken);
    }
}
