
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Lookup;

public interface ILookupService
{
    Task<List<StateDto>?> GetStatesAsync(CancellationToken cancellationToken);
}
