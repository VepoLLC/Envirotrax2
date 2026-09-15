using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogVehicleService : Service<FogVehicle, FogVehicleDto>, IFogVehicleService
{
    private readonly IFogVehicleRepository _vehicleRepository;
    private readonly IRecordLogService _recordLogService;

    public FogVehicleService(IMapper mapper, IFogVehicleRepository repository, IRecordLogService recordLogService)
        : base(mapper, repository)
    {
        _vehicleRepository = repository;
        _recordLogService = recordLogService;
    }

    public async Task<IPagedData<FogVehicleDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<FogVehicle, FogVehicleDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<FogVehicle, FogVehicleDto>(Mapper);

        var items = await _vehicleRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);
        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }

    public override async Task<FogVehicleDto> UpdateAsync(FogVehicleDto dto)
    {
        var model = MapToModel(dto)!;
        var saved = await _vehicleRepository.UpdateVehicleAsync(model);

        if (saved.Model == null)
        {
            throw new InvalidOperationException($"Vehicle {dto.Id} not found.");
        }

        if (saved.Changes.Length > 0)
        {
            await _recordLogService.AddAsync(RecordLogTableNames.FogVehicles, saved.Model.Id, null, RecordLogType.Edit, saved.Changes, professionalId: saved.Model.ProfessionalId);
        }

        return MapToDto(saved.Model)!;
    }

    public override async Task<FogVehicleDto?> DeleteAsync(int id)
    {
        var deleted = await base.DeleteAsync(id);

        if (deleted != null)
        {
            await _recordLogService.AddAsync(RecordLogTableNames.FogVehicles, deleted.Id, null, RecordLogType.Delete,
                $"Deleted vehicle — LicensePlateNumber: '{deleted.LicensePlateNumber}', StickerNumber: '{deleted.StickerNumber}'", professionalId: deleted.Professional?.Id);
        }

        return deleted;
    }
}
