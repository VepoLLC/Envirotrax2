
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

    protected override IQueryable<ProfessionalUserLicense> GetListQuery()
    {
        return base.GetListQuery()
            .Include(license => license.LicenseType);
    }

    protected override IQueryable<ProfessionalUserLicense> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(license => license.LicenseType);
    }

    public async Task<IEnumerable<ProfessionalUserLicense>> GetAllAsync(int userId, PageInfo pageInfo, Query query)
    {
        var paginated = await GetListQuery()
            .Where(license => license.UserId == userId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo);

        return await paginated.ToListAsync();
    }
}
