using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogVehiclePermitRepository : IRepository<FogVehiclePermit>
{
    Task<IEnumerable<FogVehicle>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<FogVehicle?> GetSearchResultByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken);

    Task<bool> HasVehicleInScopeAsync(int vehicleId, CancellationToken cancellationToken);

    Task<FogVehiclePermit> SetPermitAsync(FogVehiclePermit permit, CancellationToken cancellationToken);
}
