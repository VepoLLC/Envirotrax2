using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogVehiclePermitRepository : IRepository<FogVehiclePermit>
{
    Task<IEnumerable<FogVehicle>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<FogVehicle?> GetSearchResultByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken);

    Task<bool> HasVehicleInScopeAsync(int vehicleId, CancellationToken cancellationToken);

    // Add-or-update in one call. IsNew tells the caller an insert happened, which has no field diff
    // for the automatic edit logging to pick up and so needs a written "record added" log instead.
    Task<(FogVehiclePermit? Permit, bool IsNew)> SetPermitAsync(FogVehiclePermit permit, CancellationToken cancellationToken);
}
