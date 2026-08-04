
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;

public interface ISiteService
{
    Task<IPagedData<SiteDto>> SearchAsync(PageInfo pageInfo, Query query, FogCompliancyStatus? fogCompliancyStatus, CancellationToken cancellationToken);

    Task<SiteDetailDto?> GetByIdAsync(int siteId, CancellationToken cancellationToken);

    Task UpdateAsync(int siteId, SiteUpdateDto dto, CancellationToken cancellationToken);

    Task UpdateGisAsync(int siteId, SiteGisUpdateDto dto, CancellationToken cancellationToken);
}
