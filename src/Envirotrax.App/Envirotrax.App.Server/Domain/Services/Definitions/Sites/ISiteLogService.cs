using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Sites;

public interface ISiteLogService
{
    Task<IPagedData<SiteLogDto>> GetBySiteAsync(int siteId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<SiteLogDto>> GetBySitesAsync(IEnumerable<int> siteIds, CancellationToken cancellationToken);
    Task<IPagedData<SiteLogDto>> GetForManagementAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<string?> GetAttachmentUrlAsync(int logId, CancellationToken cancellationToken);
    Task<SiteLogDto> AddAsync(int siteId, SiteLogDto dto, Stream? fileStream, string? fileName);
    Task<SiteLogDto?> UpdateAsync(SiteLogDto dto, Stream? fileStream, string? fileName);
    Task<bool> DeleteAsync(int id);
}
