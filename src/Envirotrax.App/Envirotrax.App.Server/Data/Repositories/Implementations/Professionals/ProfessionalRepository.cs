
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalRepository : Repository<Professional>, IProfessionalRepository
{
    private ITenantProvidersService _tenantProvider;

    public ProfessionalRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    public async Task<IEnumerable<Professional>> GetAllMyAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await DbContext
            .ProfessionalUsers
            .IgnoreQueryFilters()
            .Where(proUser => proUser.UserId == _tenantProvider.UserId)
            .Select(proUser => proUser.Professional!)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}