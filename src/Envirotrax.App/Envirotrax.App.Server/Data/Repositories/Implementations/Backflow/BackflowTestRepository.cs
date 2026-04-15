using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowTestRepository : Repository<BackflowTest>, IBackflowTestRepository
{
    public BackflowTestRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<BackflowTest> GetListQuery()
    {
        return base.GetListQuery()
            .Include(bt => bt.Site);
    }

    protected override IQueryable<BackflowTest> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(bt => bt.Site);
    }

    public override Task<IEnumerable<BackflowTest>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowTest.Id)] = SortOperator.Asc;
        }
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }
}
