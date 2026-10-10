using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog
{
    public class FogInspectorService : Service<Professional, ProfessionalDto>, IFogInspectorService
    {
        private readonly IFogInspectorRepository _inspectorRepository;

        public FogInspectorService(IMapper mapper, IFogInspectorRepository repository)
            : base(mapper, repository)
        {
            _inspectorRepository = repository;
        }

        public async Task<IPagedData<ProfessionalDto>> SearchAsync(FogInspectorSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            query.Filter = query.ConvertFilterProperties<Professional, ProfessionalDto>(Mapper);
            query.Sort = query.ConvertSortProperties<Professional, ProfessionalDto>(Mapper);

            var items = await _inspectorRepository.SearchAsync(criteria, pageInfo, query, cancellationToken);

            return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
        }
    }
}
