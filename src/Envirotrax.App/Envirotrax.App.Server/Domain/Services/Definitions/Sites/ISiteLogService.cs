using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Sites;

public interface ISiteLogService
{
    Task<IPagedData<SiteLogDto>> GetBySiteAsync(int siteId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<SiteLogDto> AddAsync(int siteId, SiteLogDto dto, Stream? fileStream, string? fileName);
    Task<SiteLogDto?> UpdateAsync(SiteLogDto dto, Stream? fileStream, string? fileName, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
