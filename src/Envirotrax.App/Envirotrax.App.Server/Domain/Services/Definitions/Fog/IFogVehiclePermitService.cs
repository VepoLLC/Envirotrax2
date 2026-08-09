using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogVehiclePermitService : IService<FogVehiclePermitDto>
{
    Task<IPagedData<FogVehiclePermitSearchDto>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<FogVehiclePermitSearchDto?> SetPermitAsync(int vehicleId, FogVehiclePermitDto dto, CancellationToken cancellationToken);
}
