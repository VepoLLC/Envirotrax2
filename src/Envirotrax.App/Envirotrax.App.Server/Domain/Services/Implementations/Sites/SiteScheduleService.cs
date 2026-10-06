using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

public class SiteScheduleService : Service<SiteSchedule, SiteScheduleDto>, ISiteScheduleService
{
    private readonly ISiteScheduleRepository _scheduleRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IAuthService _authService;

    public SiteScheduleService(
        IMapper mapper,
        ISiteScheduleRepository repository,
        ISiteRepository siteRepository,
        IProfessionalRepository professionalRepository,
        IAuthService authService)
        : base(mapper, repository)
    {
        _scheduleRepository = repository;
        _siteRepository = siteRepository;
        _professionalRepository = professionalRepository;
        _authService = authService;
    }

    public async Task<SiteScheduleDto?> GetMyAsync(int siteId, CancellationToken cancellationToken)
    {
        var professionalType = await GetMyProfessionalTypeAsync(cancellationToken);

        if (professionalType == null)
        {
            return null;
        }

        var schedule = await _scheduleRepository.GetMyAsync(siteId, _authService.UserId, professionalType.Value, cancellationToken);

        return MapToDto(schedule);
    }

    public async Task<IEnumerable<SiteScheduleDto>> GetMyBySiteIdsAsync(IEnumerable<int> siteIds, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetMyBySiteIdsAsync(siteIds, _authService.UserId, professionalType, cancellationToken);

        return schedules.Select(s => MapToDto(s)!);
    }

    public async Task<SiteScheduleDto?> SetMyAsync(int siteId, SiteScheduleDto dto, CancellationToken cancellationToken)
    {
        var professionalType = await GetMyProfessionalTypeAsync(cancellationToken);

        if (professionalType == null || !await _siteRepository.ExistsAsync(siteId, cancellationToken))
        {
            return null;
        }

        var schedule = MapToModel(dto)!;

        schedule.Id = 0;
        schedule.SiteId = siteId;
        schedule.UserId = _authService.UserId;
        schedule.ProfessionalType = professionalType.Value;

        var saved = await _scheduleRepository.SetAsync(schedule);

        return MapToDto(saved);
    }

    public async Task<bool> ClearMyAsync(int siteId, CancellationToken cancellationToken)
    {
        var professionalType = await GetMyProfessionalTypeAsync(cancellationToken);

        if (professionalType == null)
        {
            return false;
        }

        return await _scheduleRepository.ClearAsync(siteId, _authService.UserId, professionalType.Value);
    }

    public async Task<ProfessionalType?> GetMyProfessionalTypeAsync(CancellationToken cancellationToken)
    {
        var professional = await _professionalRepository.GetNoIncludesAsync(_authService.ProfessionalId, cancellationToken);

        if (professional == null)
        {
            return null;
        }

        if (professional.HasBackflowTesting && _authService.HasAnyRole(RoleDefinitions.Professionals.BackflowTester))
        {
            return ProfessionalType.Bpat;
        }

        if (professional.HasCsiInspection && _authService.HasAnyRole(RoleDefinitions.Professionals.CsiInspector))
        {
            return ProfessionalType.CsiInspector;
        }

        if (professional.HasFogInspection && _authService.HasAnyRole(RoleDefinitions.Professionals.FogInspector))
        {
            return ProfessionalType.FogInspector;
        }

        if (professional.HasFogTransportation && _authService.HasAnyRole(RoleDefinitions.Professionals.FogTransporter))
        {
            return ProfessionalType.FogTransporter;
        }

        return null;
    }
}
