using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Backflow;

public class BackflowGaugeService : IBackflowGaugeService
{
    private const string BaseUrl = "/api/admin/backflow/gauges";

    private readonly IEnvirotraxApiClient _apiClient;

    public BackflowGaugeService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<WaterSupplierGaugeDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<WaterSupplierGaugeDto>(BaseUrl, pageInfo, query, cancellationToken);
    }

    public Task<BackflowGaugeDto?> UpdateAsync(int id, BackflowGaugeDto gauge)
    {
        return _apiClient.PutAsync<BackflowGaugeDto, BackflowGaugeDto>($"{BaseUrl}/{id}", gauge, CancellationToken.None);
    }

    public Task<string?> GetFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<string>($"{BaseUrl}/{id}/file-url", cancellationToken);
    }
}
