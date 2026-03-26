
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals.Licenses;

public class ProfessionalUserLicenseRepository : Repository<ProfessionalUserLicense>, IProfessionalUserLicenseRepository
{
    public ProfessionalUserLicenseRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<ProfessionalUserLicense>> GetAllAsync(int userId, PageInfo pageInfo, Query query)
    {
        var paginated = await DbContext
            .ProfessionalUserLicenses
            .Where(license => license.UserId == userId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo);

        return await paginated.ToListAsync();
    }
}
