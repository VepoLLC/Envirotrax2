using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Api;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Api;

public class LegacyQueryRepository : ILegacyQueryRepository
{
    private readonly IDbContextSelector _dbContextSelector;

    public LegacyQueryRepository(IDbContextSelector dbContextSelector)
    {
        _dbContextSelector = dbContextSelector;
    }

    public IQueryable<Site> QuerySites()
    {
        return _dbContextSelector.Current.Sites
            .IgnoreQueryFilters()
            .AsNoTracking();
    }

    public IQueryable<BackflowTest> QueryBackflowTests()
    {
        return _dbContextSelector.Current.BackflowTests
            .IgnoreQueryFilters()
            .AsNoTracking();
    }

    public async Task<List<object[]>> ToListAsync(IQueryable<object[]> query, CancellationToken cancellationToken)
    {
        return await query.ToListAsync(cancellationToken);
    }
}
