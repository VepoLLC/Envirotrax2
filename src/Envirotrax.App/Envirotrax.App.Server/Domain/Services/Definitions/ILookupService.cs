
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.Services.Definitions;

public interface ILookupService
{
    Task<List<StateDto>> GetStatesAsync();
}