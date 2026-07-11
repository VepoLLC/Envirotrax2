using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogVehicleRepository : IRepository<FogVehicle>
{
    Task<IReadOnlyList<FogLookupItemDto>> GetAsOptionsAsync(CancellationToken ct);
}
