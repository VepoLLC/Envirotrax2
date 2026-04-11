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

        protected override IQueryable<Professional> GetListQuery()
        {
            return base.GetListQuery()
                .Include(p => p.State);
        }

        protected override IQueryable<Professional> GetDetailsQuery()
        {
            return Entity.AsNoTracking().Include(p => p.State);
        }
    }
}
