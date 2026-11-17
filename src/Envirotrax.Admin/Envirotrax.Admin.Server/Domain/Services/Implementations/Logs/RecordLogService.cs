using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Logs;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Logs;

public class RecordLogService : IRecordLogService
{
    private const string BaseUrl = "/api/admin/record-logs";

    private readonly IEnvirotraxApiClient _apiClient;

    public RecordLogService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<RecordLogDto>> SearchAsync(RecordLogSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<RecordLogDto>(BaseUrl, pageInfo, query, criteria.ToParameters(), cancellationToken);
    }
}
