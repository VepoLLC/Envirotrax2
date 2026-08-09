
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionService : ICsiInspectionService
{
    private const string BaseUrl = "/api/admin/csi/inspections";

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

        return _apiClient.GetAsync<CsiInspectionDto>(BaseUrl, pageInfo, query, additionalParameters, cancellationToken);
    }

    public Task<CsiInspectionDetailsDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<CsiInspectionDetailsDto>($"{BaseUrl}/{id}", cancellationToken);
    }

    public Task<CsiInspectionDetailsDto?> UpdateAsync(int id, CsiInspectionUpdateRequest request, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<CsiInspectionUpdateRequest, CsiInspectionDetailsDto>($"{BaseUrl}/{id}", request, cancellationToken);
    }

    public Task<CsiInspectionCountsDto?> GetCountsAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<CsiInspectionCountsDto>($"{BaseUrl}/{id}/counts", cancellationToken);
    }

    public Task<List<CsiInspectionAssemblyDto>?> GetAssembliesAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<CsiInspectionAssemblyDto>>($"{BaseUrl}/{id}/assemblies", cancellationToken);
    }

    public Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<RecordLogDto>>($"{BaseUrl}/{id}/logs", cancellationToken);
    }

    public Task<List<CsiInspectionImageDto>?> GetImagesAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<CsiInspectionImageDto>>($"{BaseUrl}/{id}/images", cancellationToken);
    }

    public Task<CsiInspectionImageDto?> AddImageAsync(int id, Stream fileStream, string fileName, string? description, CancellationToken cancellationToken)
    {
        return _apiClient.PostFileAsync<CsiInspectionImageDto>($"{BaseUrl}/{id}/images", fileStream, fileName, description, cancellationToken);
    }

    public Task DeleteImageAsync(int id, int imageId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{id}/images/{imageId}", cancellationToken);
    }
}
