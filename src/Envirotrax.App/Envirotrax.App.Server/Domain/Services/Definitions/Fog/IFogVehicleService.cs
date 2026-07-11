using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogVehicleService : IService<FogVehicleDto>
{
    Task<IReadOnlyList<FogLookupItemDto>> GetAsOptionsAsync(CancellationToken ct);
}
