using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi
{
    public class CsiInspectorRepository : Repository<Professional>, ICsiInspectorRepository
    {
        public CsiInspectorRepository(IDbContextSelector dbContextSelector)
            : base(dbContextSelector)
        {
        }

        public async Task<Professional?> GetWithStateAsync(int id, CancellationToken cancellationToken)
        {
            return await DbContext.Professionals
                .AsNoTracking()
                .Include(p => p.State)
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
    }
}
