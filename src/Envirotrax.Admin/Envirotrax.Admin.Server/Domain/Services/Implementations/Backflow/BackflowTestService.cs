using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;
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

    public Task<BackflowTestDetailsDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<BackflowTestDetailsDto>($"{BaseUrl}/{id}", cancellationToken);
    }

    public Task<BackflowTestDetailsDto?> UpdateAsync(int id, int waterSupplierId, BackflowTestUpdateRequest request, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<BackflowTestUpdateRequest, BackflowTestDetailsDto>(waterSupplierId, $"{BaseUrl}/{id}", request, cancellationToken);
    }

    public Task<BackflowTestDetailsDto?> UploadImageAsync(int id, int waterSupplierId, string imageType, Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        var formFields = new Dictionary<string, string>();

        return _apiClient.PostFileAsync<BackflowTestDetailsDto>(waterSupplierId, $"{BaseUrl}/{id}/images/{imageType}", fileStream, fileName, "file", formFields, cancellationToken);
    }

    public Task<BackflowTestCountsDto?> GetCountsAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<BackflowTestCountsDto>($"{BaseUrl}/{id}/counts", cancellationToken);
    }

    public Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<RecordLogDto>>($"{BaseUrl}/{id}/logs", cancellationToken);
    }

    public Task<List<SiteLogDto>?> GetSiteLogsAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<SiteLogDto>>($"{BaseUrl}/{id}/site-logs", cancellationToken);
    }
}
