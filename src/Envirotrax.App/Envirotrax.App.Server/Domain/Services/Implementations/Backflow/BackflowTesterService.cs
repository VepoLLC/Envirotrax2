using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow
{
    public class BackflowTesterService : Service<Professional, ProfessionalDto>, IBackflowTesterService
    {
        private readonly IBackflowTesterRepository _testerRepository;

        public BackflowTesterService(IMapper mapper, IBackflowTesterRepository repository)
            : base(mapper, repository)
        {
            _testerRepository = repository;
        }

        public async Task<IPagedData<ProfessionalDto>> SearchAsync(BackflowTesterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            query.Filter = query.ConvertFilterProperties<Professional, ProfessionalDto>(Mapper);
            query.Sort = query.ConvertSortProperties<Professional, ProfessionalDto>(Mapper);

            var items = await _testerRepository.SearchAsync(criteria, pageInfo, query, cancellationToken);

            return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
        }
    }
}
