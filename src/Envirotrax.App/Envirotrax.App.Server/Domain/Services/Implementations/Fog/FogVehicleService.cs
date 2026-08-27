using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogVehicleService : Service<FogVehicle, FogVehicleDto>, IFogVehicleService
{
    private readonly IFogVehicleRepository _vehicleRepository;

    public FogVehicleService(IMapper mapper, IFogVehicleRepository repository)
        : base(mapper, repository)
    {
        _vehicleRepository = repository;
    }

    public async Task<IPagedData<FogVehicleDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<FogVehicle, FogVehicleDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<FogVehicle, FogVehicleDto>(Mapper);

        var items = await _vehicleRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);
        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }
}
