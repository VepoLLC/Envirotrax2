using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Sites;

public interface ISiteService : IService<Site, SiteDto>
{
    Task<IPagedData<SiteDto>> SearchAsync(PageInfo pageInfo, Query query, FogCompliancyStatus? fogCompliancyStatus, CancellationToken cancellationToken);
    Task<IEnumerable<SiteDto>> GetAllPendingGeocodingAsync(int batchSize);
    Task<SiteDto?> GeocodeAsync(int siteId, bool assignGisArea, CancellationToken cancellationToken);
    Task UpdateGisDataAsync(int siteId, UpdateSiteGisDataDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateFromAdminAsync(int siteId, SiteDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateWaterSupplierAsync(int siteId, UpdateSiteWaterSupplierDto dto);
    Task<IPagedData<CsiComplianceSiteDto>> GetCsiComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task UpdateCsiAssignmentAsync(int siteId, int? userId);
    Task UpdateBackflowAssignmentAsync(int siteId, int? userId);
    Task<IPagedData<FogTripTicketComplianceSiteDto>> GetFogTripTicketComplianceAsync(PageInfo pageInfo, Query query, DateTime? dueDateFrom, DateTime? dueDateTo, bool sortDescending, CancellationToken cancellationToken);
    Task UpdateFogAssignmentAsync(int siteId, int? userId);
    Task<IEnumerable<SiteDto>> GetAllPendingRenewalAsync(int batchSize, CancellationToken cancellationToken);
}
