using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogDisposalSiteService : IService<FogDisposalSiteDto>
{
    Task<IPagedData<FogDisposalSiteDto>> GetActiveAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
