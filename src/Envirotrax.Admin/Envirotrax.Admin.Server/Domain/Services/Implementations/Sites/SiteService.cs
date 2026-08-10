
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Sites;

public class SiteService : ISiteService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public SiteService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<SiteDto>> SearchAsync(PageInfo pageInfo, Query query, FogCompliancyStatus? fogCompliancyStatus, CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>();

        if (fogCompliancyStatus.HasValue)
        {
            additionalParameters["fogCompliancyStatus"] = ((int)fogCompliancyStatus.Value).ToString();
        }

        return _apiClient.GetAsync<SiteDto>("/api/admin/sites", pageInfo, query, additionalParameters, cancellationToken);
    }

    public Task<SiteDetailDto?> GetByIdAsync(int siteId, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<SiteDetailDto>($"/api/admin/sites/{siteId}", cancellationToken);
    }

    public Task UpdateAsync(int siteId, SiteUpdateDto dto, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<SiteUpdateDto, object>($"/api/admin/sites/{siteId}", dto, cancellationToken);
    }

    public Task UpdateGisAsync(int siteId, SiteGisUpdateDto dto, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<SiteGisUpdateDto, object>($"/api/admin/sites/{siteId}/gis-data", dto, cancellationToken);
    }
}
