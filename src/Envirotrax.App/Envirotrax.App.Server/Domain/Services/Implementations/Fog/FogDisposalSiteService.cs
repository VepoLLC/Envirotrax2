using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogDisposalSiteService : Service<FogDisposalSite, FogDisposalSiteDto>, IFogDisposalSiteService
{
    private readonly IFogDisposalSiteRepository _repository;

    public FogDisposalSiteService(IMapper mapper, IFogDisposalSiteRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<IPagedData<FogDisposalSiteDto>> GetActiveAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<FogDisposalSite, FogDisposalSiteDto>(Mapper);

        var sites = await _repository.GetActiveAsync(pageInfo, query, cancellationToken);

        return Mapper
            .Map<IEnumerable<FogDisposalSite>, IEnumerable<FogDisposalSiteDto>>(sites)
            .ToPagedData(pageInfo);
    }
}
