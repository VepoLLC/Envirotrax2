using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogTransporterDisposalSiteService : IService<FogTransporterDisposalSiteDto>
{
    Task<IPagedData<FogDisposalSiteCandidateDto>> GetAvailableAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<FogDisposalSiteCandidateDto> SetRegistrationAsync(int disposalSiteId, bool isActive, CancellationToken cancellationToken);
}
