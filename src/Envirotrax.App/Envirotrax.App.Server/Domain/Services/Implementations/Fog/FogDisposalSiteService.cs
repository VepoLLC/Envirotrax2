using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogDisposalSiteService : Service<FogDisposalSite, FogDisposalSiteDto>, IFogDisposalSiteService
{
    private readonly IFogDisposalSiteRepository _disposalSiteRepository;
    private readonly IRecordLogService _recordLogService;

    public FogDisposalSiteService(IMapper mapper, IFogDisposalSiteRepository repository, IRecordLogService recordLogService)
        : base(mapper, repository)
    {
        _disposalSiteRepository = repository;
        _recordLogService = recordLogService;
    }

    public override async Task<FogDisposalSiteDto> UpdateAsync(FogDisposalSiteDto dto)
    {
        var model = MapToModel(dto)!;
        var saved = await _disposalSiteRepository.UpdateSiteAsync(model);

        if (saved == null)
        {
            throw new InvalidOperationException($"Disposal site {dto.Id} not found.");
        }

        return MapToDto(saved)!;
    }

    public override async Task<FogDisposalSiteDto?> DeleteAsync(int id)
    {
        var deleted = await base.DeleteAsync(id);

        if (deleted != null)
        {
            await _recordLogService.AddAsync(RecordLogTableNames.FogDisposalSites, deleted.Id, null, RecordLogType.Delete,
                $"Deleted disposal site — Name: '{deleted.Name}', RegistrationNumber: '{deleted.RegistrationNumber}'");
        }

        return deleted;
    }
}
