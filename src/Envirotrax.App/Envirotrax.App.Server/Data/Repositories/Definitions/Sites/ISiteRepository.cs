using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Sites;

public interface ISiteRepository : IRepository<Site>
{
    Task<IEnumerable<Site>> SearchAsync(PageInfo pageInfo, Query query, bool? fogCompliant, CancellationToken cancellationToken);
    Task<IEnumerable<Site>> GetAllPendingGeocodingAsync(int batchSize);
    Task UpdateGisCoordinatesAsync(Site site);
    Task UpdateManualGisDataAsync(int siteId, double? latitude, double? longitude, GisStatusType status);
    Task<IEnumerable<Site>> GetCsiComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<Site>> GetFogInspectionComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<Site>> GetFogPermitComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task UpdateCsiAssignmentAsync(int siteId, int? userId, DateTime? assignmentDate);
    Task UpdateBackflowAssignmentAsync(int siteId, int? userId, DateTime? assignmentDate);
    Task<IEnumerable<Site>> GetFogTripTicketComplianceAsync(PageInfo pageInfo, Query query, DateTime? dueDateFrom, DateTime? dueDateTo, bool sortDescending, CancellationToken cancellationToken);
    Task UpdateFogAssignmentAsync(int siteId, int? userId, DateTime? assignmentDate);
    Task ClearNeedsRenewalCheckAsync(int siteId);
    Task<IEnumerable<Site>> GetAllPendingRenewalAsync(int batchSize);
}
