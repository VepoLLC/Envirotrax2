using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogTransporterDisposalSiteService : Service<FogTransporterDisposalSite, FogTransporterDisposalSiteDto>, IFogTransporterDisposalSiteService
{
    private readonly IFogTransporterDisposalSiteRepository _repository;

    public FogTransporterDisposalSiteService(IMapper mapper, IFogTransporterDisposalSiteRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<IPagedData<FogDisposalSiteDto>> GetRegisteredDisposalSitesAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<FogDisposalSite, FogDisposalSiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<FogDisposalSite, FogDisposalSiteDto>(Mapper);

        var sites = await _repository.GetRegisteredDisposalSitesAsync(pageInfo, query, cancellationToken);

        return Mapper
            .Map<IEnumerable<FogDisposalSite>, IEnumerable<FogDisposalSiteDto>>(sites)
            .ToPagedData(pageInfo);
    }

    public async Task<FogTransporterDisposalSiteDto> SetRegistrationAsync(int disposalSiteId, bool isActive, CancellationToken cancellationToken)
    {
        var registration = new FogTransporterDisposalSite
        {
            DisposalSiteId = disposalSiteId,
            IsActive = isActive
        };

        var result = await _repository.SetRegistrationAsync(registration, cancellationToken);

        return Mapper.Map<FogTransporterDisposalSite, FogTransporterDisposalSiteDto>(result);
    }
}
