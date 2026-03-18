using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi;

public class CsiInspectionRepository : Repository<CsiInspection>, ICsiInspectionRepository
{
    public CsiInspectionRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<CsiInspection> GetListQuery()
    {
        return base.GetListQuery()
            .Include(c => c.Site);
    }

    protected override IQueryable<CsiInspection> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(c => c.Site);
    }
}
