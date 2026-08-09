using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogVehiclePermitService : Service<FogVehiclePermit, FogVehiclePermitDto>, IFogVehiclePermitService
{
    private readonly IFogVehiclePermitRepository _permitRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public FogVehiclePermitService(
        IMapper mapper,
        IFogVehiclePermitRepository repository,
        ITimeZoneHelperService timeZoneHelper)
        : base(mapper, repository)
    {
        _permitRepository = repository;
        _timeZoneHelper = timeZoneHelper;
    }

    public async Task<IPagedData<FogVehiclePermitSearchDto>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<FogVehiclePermitSearchResult, FogVehiclePermitSearchDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogVehiclePermitSearchResult, FogVehiclePermitSearchDto>(Mapper);

        var results = await _permitRepository.SearchAsync(pageInfo, query, cancellationToken);

        var now = _timeZoneHelper.GetUserLocalTime();
        var dtos = results.Select(result => MapToDto(result, now)).ToList();

        return dtos.ToPagedData(pageInfo);
    }

    public async Task<FogVehiclePermitSearchDto?> SetPermitAsync(int vehicleId, FogVehiclePermitDto dto, CancellationToken cancellationToken)
    {
        if (!await _permitRepository.HasVehicleInScopeAsync(vehicleId, cancellationToken))
        {
            return null;
        }

        var permit = MapToModel(dto)!;
        permit.VehicleId = vehicleId;

        await _permitRepository.SetPermitAsync(permit, cancellationToken);

        var result = await _permitRepository.GetSearchResultByVehicleIdAsync(vehicleId, cancellationToken);

        return result != null
            ? MapToDto(result, _timeZoneHelper.GetUserLocalTime())
            : null;
    }

    private FogVehiclePermitSearchDto MapToDto(FogVehiclePermitSearchResult result, DateTime now)
    {
        var dto = Mapper.Map<FogVehiclePermitSearchDto>(result)!;
        dto.InspectionDueStatus = ComputeInspectionDueStatus(dto.InspectionDueDate, now);

        return dto;
    }

    private static FogVehicleInspectionDueStatus ComputeInspectionDueStatus(DateTime? inspectionDueDate, DateTime now)
    {
        if (!inspectionDueDate.HasValue)
        {
            return FogVehicleInspectionDueStatus.None;
        }

        if (inspectionDueDate.Value < now)
        {
            return FogVehicleInspectionDueStatus.PastDue;
        }

        return FogVehicleInspectionDueStatus.Current;
    }
}
