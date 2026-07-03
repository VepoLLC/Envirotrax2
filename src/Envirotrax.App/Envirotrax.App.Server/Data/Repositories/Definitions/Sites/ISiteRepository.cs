using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Sites;

public interface ISiteRepository : IRepository<Site>
{
    Task<IEnumerable<Site>> SearchAsync(PageInfo pageInfo, Query query, bool? fogCompliant, CancellationToken cancellationToken);
    Task<IEnumerable<Site>> GetAllPendingGeocodingAsync(int batchSize);
    Task UpdateGisCoordinatesAsync(Site site);
    Task UpdateManualGisDataAsync(int siteId, double? latitude, double? longitude, GisStatusType status);
}
