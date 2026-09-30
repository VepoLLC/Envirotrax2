using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;
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

    public Task<IPagedData<FogInspectionDto>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<FogInspectionDto>(BaseUrl, pageInfo, query, cancellationToken);
    }

    public Task<FogInspectionDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<FogInspectionDto>($"{BaseUrl}/{id}", cancellationToken);
    }

    public Task<FogInspectionDto?> UpdateAsync(int id, int waterSupplierId, FogInspectionUpdateRequest request, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<FogInspectionUpdateRequest, FogInspectionDto>(waterSupplierId, $"{BaseUrl}/{id}", request, cancellationToken);
    }

    public Task<FogInspectionDto?> UploadImageAsync(int id, int waterSupplierId, string imageType, Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        var formFields = new Dictionary<string, string>();

        return _apiClient.PostFileAsync<FogInspectionDto>(waterSupplierId, $"{BaseUrl}/{id}/images/{imageType}", fileStream, fileName, "file", formFields, cancellationToken);
    }

    public Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<RecordLogDto>>($"{BaseUrl}/{id}/logs", cancellationToken);
    }
}
