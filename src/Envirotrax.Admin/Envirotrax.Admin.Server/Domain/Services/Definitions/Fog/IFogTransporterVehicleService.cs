using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

public interface IFogTransporterVehicleService
{
    Task<IPagedData<FogVehicleDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<FogVehicleDto?> AddAsync(int professionalId, FogVehicleDto vehicle, CancellationToken cancellationToken);

    Task<FogVehicleDto?> UpdateAsync(int professionalId, int vehicleId, FogVehicleDto vehicle, CancellationToken cancellationToken);

    Task DeleteAsync(int professionalId, int vehicleId, CancellationToken cancellationToken);
}
