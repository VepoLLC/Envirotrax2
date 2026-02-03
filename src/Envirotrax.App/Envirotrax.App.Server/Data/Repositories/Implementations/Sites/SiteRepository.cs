using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public class SiteRepository : Repository<Site>, ISiteRepository
{
    public SiteRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<Site> GetListQuery()
    {
        return base.GetListQuery()
            .Include(s => s.State)
            .Include(s => s.MailingState);
    }

    protected override IQueryable<Site> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(s => s.State)
            .Include(s => s.MailingState);
    }
}
