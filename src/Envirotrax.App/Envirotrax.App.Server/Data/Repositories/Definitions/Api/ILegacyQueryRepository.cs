using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Api;

/// <summary>
/// Base queryables for the legacy API. Both ignore the ambient tenant query filters: the legacy
/// caller has no tenant claim, so those filters would silently return nothing. Scoping is applied
/// explicitly by the caller from the authenticated account's supplier scope.
/// </summary>
public interface ILegacyQueryRepository
{
    IQueryable<Site> QuerySites();

    IQueryable<BackflowTest> QueryBackflowTests();

    Task<List<object[]>> ToListAsync(IQueryable<object[]> query, CancellationToken cancellationToken);
}
