using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;

public interface ISiteLogService
{
    Task<IPagedData<SiteLogDto>> GetBySiteAsync(int siteId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<SiteLogDto?> GetAsync(int siteId, int id, CancellationToken cancellationToken);

    Task<SiteLogDto?> AddAsync(int siteId, int waterSupplierId, SiteLogDto dto);

    Task<SiteLogDto?> UpdateAsync(int siteId, int id, int waterSupplierId, SiteLogDto dto);

    Task DeleteAsync(int siteId, int id);
}
