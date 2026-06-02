
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

    public async Task<IEnumerable<ProfessionalUserLicense>> GetBpatLicensesForProfessionalAsync(int professionalId, CancellationToken cancellationToken)
    {
        return await DbContext.ProfessionalUserLicenses
            .AsNoTracking()
            .Include(l => l.LicenseType)
            .Where(l => l.ProfessionalId == professionalId && l.ProfessionalType == ProfessionalType.Bpat)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalUserLicense>> GetAllByWaterSupplierAsync(PageInfo pageInfo, Query query, string? licenseFilter, CancellationToken cancellationToken)
    {
        var baseQuery = DbContext.ProfessionalUserLicenses
            .AsNoTracking()
            .Include(l => l.LicenseType)
            .Include(l => l.User)
            .Include(l => l.Professional)
            .Include(l => l.ProfessionalUser)
            .Where(l => DbContext.ProfessionalWaterSuppliers.Any(pws => pws.ProfessionalId == l.ProfessionalId));

        baseQuery = ApplyLicenseFilter(baseQuery, licenseFilter);

        if (query.Sort.IsNullOrEmpty())
            query.Sort[nameof(ProfessionalUserLicense.Id)] = SortOperator.Asc;

        var paginated = await baseQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByWaterSupplierAsync(string? licenseFilter, CancellationToken cancellationToken)
    {
        var baseQuery = DbContext.ProfessionalUserLicenses
            .AsNoTracking()
            .Where(l => DbContext.ProfessionalWaterSuppliers.Any(pws => pws.ProfessionalId == l.ProfessionalId));

        baseQuery = ApplyLicenseFilter(baseQuery, licenseFilter);

        return await baseQuery.CountAsync(cancellationToken);
    }

    private static IQueryable<ProfessionalUserLicense> ApplyLicenseFilter(IQueryable<ProfessionalUserLicense> query, string? licenseFilter)
    {
        var now = DateTime.UtcNow;
        var firstDayThisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);
        return licenseFilter switch
        {
            "unverified" => query.Where(l => l.ExpirationDate == null),
            "expired" => query.Where(l => l.ExpirationDate != null
                && l.ExpirationDate >= firstDayLastMonth
                && l.ExpirationDate < firstDayThisMonth),
            "expiring" => query.Where(l => l.ExpirationDate != null && l.ExpirationDate >= firstDayThisMonth && l.ExpirationDate < firstDayThisMonth.AddMonths(1)),
            _ => query
        };
    }

    public async Task<ProfessionalUserLicense> UpdateForWaterSupplierAsync(int id, string licenseNumber, string? contactName, DateTime? expirationDate, CancellationToken cancellationToken)
    {
        var license = await DbContext.ProfessionalUserLicenses
            .Include(l => l.LicenseType)
            .Include(l => l.User)
            .Include(l => l.Professional)
            .Include(l => l.ProfessionalUser)
            .FirstOrDefaultAsync(l => l.Id == id
                && DbContext.ProfessionalWaterSuppliers.Any(pws => pws.ProfessionalId == l.ProfessionalId), cancellationToken)
            ?? throw new InvalidOperationException($"License {id} not found for current water supplier.");

        license.LicenseNumber = licenseNumber;
        license.ExpirationDate = expirationDate;

        if (license.ProfessionalUser != null)
            license.ProfessionalUser.ContactName = contactName;

        await DbContext.SaveChangesAsync(cancellationToken);
        return license;
    }

    public async Task DeleteForWaterSupplierAsync(int id, CancellationToken cancellationToken)
    {
        var license = await DbContext.ProfessionalUserLicenses
            .FirstOrDefaultAsync(l => l.Id == id
                && DbContext.ProfessionalWaterSuppliers.Any(pws => pws.ProfessionalId == l.ProfessionalId), cancellationToken)
            ?? throw new InvalidOperationException($"License {id} not found for current water supplier.");

        DbContext.ProfessionalUserLicenses.Remove(license);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
