using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogVehicleService : IService<FogVehicleDto>
{
    Task<IPagedData<FogVehicleDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
