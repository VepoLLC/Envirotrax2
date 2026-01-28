
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalService : Service<Professional, ProfessionalDto>, IProfessionalService
{
    private readonly IProfessionalRepository _professionalRepository;

    public ProfessionalService(IMapper mapper, IProfessionalRepository repository)
        : base(mapper, repository)
    {
        _professionalRepository = repository;
    }

    public async Task<IPagedData<ProfessionalDto>> GetAllMyAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Professional, ProfessionalDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Professional, ProfessionalDto>(Mapper);

        var professionals = await _professionalRepository.GetAllMyAsync(pageInfo, query, cancellationToken);
        var professionalsDto = Mapper.Map<IEnumerable<Professional>, IEnumerable<ProfessionalDto>>(professionals);

        return professionalsDto.ToPagedData(pageInfo);
    }
}