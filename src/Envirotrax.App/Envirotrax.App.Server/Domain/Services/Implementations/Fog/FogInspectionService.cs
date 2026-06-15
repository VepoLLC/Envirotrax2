using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogInspectionService : Service<FogInspection, FogInspectionDto>, IFogInspectionService
{
    private readonly IFogInspectionRepository _repository;

    public FogInspectionService(IMapper mapper, IFogInspectionRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<IPagedData<FogInspectionDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<FogInspection, FogInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogInspection, FogInspectionDto>(Mapper);

        var inspections = await _repository.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);

        return inspections.Select(m => Mapper.Map<FogInspectionDto>(m)!).ToPagedData(pageInfo);
    }
}
