
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalService : Service<Professional, ProfessionalDto>, IProfessionalService
{
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IAuthService _authService;

    public ProfessionalService(
        IMapper mapper,
        IProfessionalRepository repository,
        IProfessionalUserRepository professionalUserRepository,
        IAuthService authService)
        : base(mapper, repository)
    {
        _professionalRepository = repository;
        _professionalUserRepository = professionalUserRepository;
        _authService = authService;
    }

    public async Task<IPagedData<ProfessionalDto>> GetAllMyAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Professional, ProfessionalDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Professional, ProfessionalDto>(Mapper);

        var professionals = await _professionalRepository.GetAllMyAsync(pageInfo, query, cancellationToken);
        var professionalsDto = Mapper.Map<IEnumerable<Professional>, IEnumerable<ProfessionalDto>>(professionals);

        return professionalsDto.ToPagedData(pageInfo);
    }

    public async Task<ProfessionalDto?> GetLoggedInProfessionalAsync(CancellationToken cancellationToken)
    {
        return await GetAsync(_authService.ProfessionalId, cancellationToken);
    }

    public async Task<ProfessionalDto> AddMyAsync(ProfessionalDto professional)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var added = await AddAsync(professional);

            await _professionalUserRepository.AddAsync(new()
            {
                ProfessionalId = added.Id,
                UserId = _authService.UserId
            });

            scope.Complete();
            return added;
        }
    }
}