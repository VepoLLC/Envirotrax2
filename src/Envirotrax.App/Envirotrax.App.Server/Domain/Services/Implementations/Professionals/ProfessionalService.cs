
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalService : Service<Professional, ProfessionalDto>, IProfessionalService
{
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IAuthService _authService;
    private readonly IProfessionalInsuranceRepository _insuranceRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public ProfessionalService(
        IMapper mapper,
        IProfessionalRepository repository,
        IProfessionalUserRepository professionalUserRepository,
        IAuthService authService,
        IProfessionalInsuranceRepository insuranceRepository,
        ITimeZoneHelperService timeZoneHelper)
        : base(mapper, repository)
    {
        _professionalRepository = repository;
        _professionalUserRepository = professionalUserRepository;
        _authService = authService;
        _insuranceRepository = insuranceRepository;
        _timeZoneHelper = timeZoneHelper;
    }

    public async Task<IPagedData<ProfessionalDto>> GetAllMyAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Professional, ProfessionalDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Professional, ProfessionalDto>(Mapper);

        var professionals = await _professionalRepository.GetAllMyAsync(pageInfo, query, cancellationToken);
        var professionalsDto = Mapper.Map<IEnumerable<Professional>, IEnumerable<ProfessionalDto>>(professionals);

        return professionalsDto.ToPagedData(pageInfo);
    }

    public async Task<ProfessionalDto?> GetLoggedInProfessionalAsync()
    {
        return await GetLoggedInProfessionalAsync(CancellationToken.None);
    }

    public async Task<ProfessionalDto?> GetLoggedInProfessionalAsync(CancellationToken cancellationToken)
    {
        var dto = await GetAsync(_authService.ProfessionalId, cancellationToken);
        if (dto != null)
        {
            var insurance = await _insuranceRepository.GetCurrentForProfessionalAsync(_authService.ProfessionalId, cancellationToken);
            if (insurance != null)
            {
                var localTime = _timeZoneHelper.GetUserLocalTime();
                dto.InsuranceExpirationType = localTime > insurance.ExpirationDate
                    ? ExpirationType.Expired
                    : localTime.AddDays(30) >= insurance.ExpirationDate
                        ? ExpirationType.AboutToExpire
                        : ExpirationType.Valid;
            }
        }
        return dto;
    }

    public async Task<ProfessionalDto> AddMyAsync(CreateProfessionalDto createProfessional)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var added = await AddAsync(createProfessional.Professional);

            await _professionalUserRepository.AddAsync(new()
            {
                ProfessionalId = added.Id,
                UserId = _authService.UserId,
                ContactName = createProfessional.User.ContactName,
                JobTitle = createProfessional.User.JobTitle,
                IsAdmin = true,
                IsBackflowTester = createProfessional.Professional.HasBackflowTesting,
                IsCsiInspector = createProfessional.Professional.HasCsiInspection,
                IsFogInspector = createProfessional.Professional.HasFogInspection,
                IsFogTransporter = createProfessional.Professional.HasFogTransportation,
                IsWiseGuy = createProfessional.Professional.HasWiseGuys
            });

            scope.Complete();
            return added;
        }
    }
}