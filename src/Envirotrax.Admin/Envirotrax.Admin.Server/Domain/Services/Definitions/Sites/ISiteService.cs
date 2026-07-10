
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;

public interface ISiteService
{
    Task<IPagedData<SiteDto>> SearchAsync(PageInfo pageInfo, Query query, FogCompliancyStatus? fogCompliancyStatus, CancellationToken cancellationToken);
}
