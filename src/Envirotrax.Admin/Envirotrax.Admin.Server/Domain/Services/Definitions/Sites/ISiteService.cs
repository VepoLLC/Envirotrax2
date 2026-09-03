
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;

public interface ISiteService
{
    Task<IPagedData<SiteDto>> SearchAsync(PageInfo pageInfo, Query query, FogCompliancyStatus? fogCompliancyStatus, CancellationToken cancellationToken);

    Task<SiteDetailDto?> GetByIdAsync(int siteId, CancellationToken cancellationToken);

    Task UpdateAsync(int siteId, int waterSupplierId, SiteUpdateDto dto, CancellationToken cancellationToken);

    Task UpdateGisAsync(int siteId, int waterSupplierId, SiteGisUpdateDto dto, CancellationToken cancellationToken);

    Task UpdateWaterSupplierAsync(int siteId, int waterSupplierId, SiteWaterSupplierUpdateDto dto, CancellationToken cancellationToken);
}
