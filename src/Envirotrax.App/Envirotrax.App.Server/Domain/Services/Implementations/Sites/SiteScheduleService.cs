using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

public class SiteScheduleService : Service<SiteSchedule, SiteScheduleDto>, ISiteScheduleService
{
    private readonly ISiteScheduleRepository _scheduleRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IAuthService _authService;

    public SiteScheduleService(
        IMapper mapper,
        ISiteScheduleRepository repository,
        ISiteRepository siteRepository,
        IAuthService authService)
        : base(mapper, repository)
    {
        _scheduleRepository = repository;
        _siteRepository = siteRepository;
        _authService = authService;
    }

    public async Task<IEnumerable<SiteScheduleDto>> GetMyBySiteIdsAsync(IEnumerable<int> siteIds, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetMyBySiteIdsAsync(siteIds, _authService.UserId, cancellationToken);

        return schedules.Select(s => MapToDto(s)!);
    }

    public async Task<SiteScheduleDto?> SetMyAsync(int siteId, SiteScheduleDto dto, CancellationToken cancellationToken)
    {
        if (!await _siteRepository.ExistsAsync(siteId, cancellationToken))
        {
            return null;
        }

        var schedule = MapToModel(dto)!;

        schedule.Id = 0;
        schedule.SiteId = siteId;
        schedule.UserId = _authService.UserId;

        var saved = await _scheduleRepository.SetAsync(schedule);

        return MapToDto(saved);
    }

    public Task<bool> ClearMyAsync(int siteId, ProfessionalType professionalType)
    {
        return _scheduleRepository.ClearAsync(siteId, _authService.UserId, professionalType);
    }
}
