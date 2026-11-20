using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Sites;

public class SiteLogService : ISiteLogService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public SiteLogService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<SiteLogDto>> GetBySiteAsync(int siteId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<SiteLogDto>($"/api/admin/sites/{siteId}/site-logs", pageInfo, query, cancellationToken);
    }

    public Task<SiteLogDto?> GetAsync(int siteId, int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<SiteLogDto>($"/api/admin/sites/{siteId}/site-logs/{id}", cancellationToken);
    }

    public Task<SiteLogDto?> AddAsync(int siteId, int waterSupplierId, SiteLogDto dto)
    {
        return _apiClient.PostAsync<SiteLogDto, SiteLogDto>(waterSupplierId, $"/api/admin/sites/{siteId}/site-logs", dto, CancellationToken.None);
    }

    public Task<SiteLogDto?> UpdateAsync(int siteId, int id, int waterSupplierId, SiteLogDto dto)
    {
        return _apiClient.PutAsync<SiteLogDto, SiteLogDto>(waterSupplierId, $"/api/admin/sites/{siteId}/site-logs/{id}", dto, CancellationToken.None);
    }

    public Task DeleteAsync(int siteId, int id)
    {
        return _apiClient.DeleteAsync<object>($"/api/admin/sites/{siteId}/site-logs/{id}", CancellationToken.None);
    }
}
