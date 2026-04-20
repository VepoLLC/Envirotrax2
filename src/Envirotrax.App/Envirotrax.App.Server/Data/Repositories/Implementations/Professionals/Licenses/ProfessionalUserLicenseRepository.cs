
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

    public Task<ProfessionalUserLicense?> GetCsiLicenseForUserAsync(int userId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        return GetListQuery()
            .Where(l => l.UserId == userId && l.ProfessionalType == ProfessionalType.CsiInspector)
            .OrderByDescending(l => l.ExpirationDate == null || l.ExpirationDate > now)
            .ThenByDescending(l => l.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalUserLicense>> GetCsiLicensesForProfessionalAsync(int professionalId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var licenses = await GetListQuery()
            .Where(l => l.ProfessionalId == professionalId && l.ProfessionalType == ProfessionalType.CsiInspector)
            .ToListAsync(cancellationToken);

        return licenses
            .GroupBy(l => l.UserId)
            .Select(g => g
                .OrderByDescending(l => l.ExpirationDate == null || l.ExpirationDate > now)
                .ThenByDescending(l => l.Id)
                .First())
            .ToList();
    }

    public async Task<IEnumerable<ProfessionalUserLicense>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await DbContext.ProfessionalUserLicenses
            .AsNoTracking()
            .Include(l => l.LicenseType)
            .Include(l => l.User)
            .Where(l => l.ProfessionalId == professionalId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
