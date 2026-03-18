using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Lookup
{
    public class LookupRepository
    {
        private readonly TenantDbContext _dbContext;
        public LookupRepository(TenantDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<List<StateDto>> GetStatesAsync()
        {
            return await _dbContext.States
                .Select(s => new StateDto 
                { 
                    Id = s.Id, 
                    Name = s.Name, 
                    Code = s.Code 
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
