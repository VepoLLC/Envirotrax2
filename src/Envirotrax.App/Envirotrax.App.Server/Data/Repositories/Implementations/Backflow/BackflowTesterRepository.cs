using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow
{
    public class BackflowTesterRepository : Repository<Professional>, IBackflowTesterRepository
    {
        public BackflowTesterRepository(IDbContextSelector dbContextSelector)
            : base(dbContextSelector)
        {
        }

        protected override IQueryable<Professional> GetListQuery()
        {
            return base.GetListQuery()
                .Where(p => p.HasBackflowTesting);
        }
    }
}
