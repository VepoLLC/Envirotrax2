using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog
{
    public class FogInspectorRepository : Repository<Professional>, IFogInspectorRepository
    {
        public FogInspectorRepository(IDbContextSelector dbContextSelector)
            : base(dbContextSelector)
        {
        }

        protected override IQueryable<Professional> GetListQuery()
        {
            return Entity.AsNoTracking()
            .Include(p => p.State)
            .Where(p => p.HasFogInspection);
        }
    }
}
