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

    public async Task<IPagedData<FogDisposalSiteCandidateDto>> GetAvailableAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<FogDisposalSiteCandidate, FogDisposalSiteCandidateDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<FogDisposalSiteCandidate, FogDisposalSiteCandidateDto>(Mapper);

        var candidates = await _repository.GetAvailableAsync(pageInfo, query, cancellationToken);

        return Mapper
            .Map<IEnumerable<FogDisposalSiteCandidate>, IEnumerable<FogDisposalSiteCandidateDto>>(candidates)
            .ToPagedData(pageInfo);
    }

    public async Task<FogDisposalSiteCandidateDto> SetRegistrationAsync(int disposalSiteId, bool isActive, CancellationToken cancellationToken)
    {
        var candidate = await _repository.SetRegistrationAsync(disposalSiteId, isActive, cancellationToken);
        return Mapper.Map<FogDisposalSiteCandidate, FogDisposalSiteCandidateDto>(candidate);
    }
}
