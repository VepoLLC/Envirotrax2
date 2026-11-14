
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

public interface IFogDisposalSiteService
{
    Task<IPagedData<FogDisposalSiteDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<FogDisposalSiteDto?> AddAsync(FogDisposalSiteDto site, CancellationToken cancellationToken);
    Task<FogDisposalSiteDto?> UpdateAsync(int id, FogDisposalSiteDto site, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken);
}
