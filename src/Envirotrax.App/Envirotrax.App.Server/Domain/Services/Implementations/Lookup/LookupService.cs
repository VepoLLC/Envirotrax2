using Envirotrax.App.Server.Data.Repositories.Implementations.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Lookup
{
    public class LookupService
    {
        private readonly LookupRepository _lookupRepository;
        public LookupService(LookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public async Task<List<StateDto>> GetStatesAsync()
        {
            return await _lookupRepository.GetStatesAsync();
        }
    }
}
