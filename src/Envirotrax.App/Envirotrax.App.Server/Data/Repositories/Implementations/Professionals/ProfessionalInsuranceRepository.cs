
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalInsuranceRepository : Repository<ProfessionalInsurance>, IProfessionalInsuranceRepository
{
    public ProfessionalInsuranceRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override void UpdateEntity(ProfessionalInsurance model)
    {
        base.UpdateEntity(model);

        // You cannot update the file path.
        DbContext.Entry(model).Property(i => i.FilePath).IsModified = false;
    }

    public async Task<IEnumerable<ProfessionalInsurance>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .Where(i => i.ProfessionalId == professionalId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalInsurance>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        return await DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .Where(i => professionalIds.Contains(i.ProfessionalId))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Used for validating a specific professional's own policies at submission time, including a
    /// sub-account's parent. ProfessionalInsurance carries the ProfessionalId query filter under
    /// ProfessionalDbContext, which would silently rewrite "give me this professional's policies" into
    /// "give me the CALLER's policies" - IgnoreQueryFilters() is load-bearing here, not decorative.
    /// </summary>
    public async Task<IReadOnlyList<ProfessionalInsurance>> GetAllForValidationAsync(int professionalId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(i => i.ProfessionalId == professionalId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalInsurance>> GetAllByWaterSupplierAsync(PageInfo pageInfo, Query query, string? insuranceFilter, CancellationToken cancellationToken)
    {
        var baseQuery = ApplyInsuranceFilter(ScopedToWaterSupplier().Include(i => i.Professional), insuranceFilter);

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(ProfessionalInsurance.Id)] = SortOperator.Asc;
        }

        var paginated = await baseQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByWaterSupplierAsync(string? insuranceFilter, CancellationToken cancellationToken)
    {
        return await ApplyInsuranceFilter(ScopedToWaterSupplier(), insuranceFilter).CountAsync(cancellationToken);
    }

    public async Task<ProfessionalInsurance> GetForWaterSupplierAsync(int id, CancellationToken cancellationToken)
    {
        return await ScopedToWaterSupplier()
            .Include(i => i.Professional)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken)
            ?? throw new InvalidOperationException($"Insurance policy {id} not found for current water supplier.");
    }

    public async Task<ProfessionalInsurance> UpdateForWaterSupplierAsync(int id, string insuranceNumber, DateTime? expirationDate, decimal? insuranceCoverage, CancellationToken cancellationToken)
    {
        var insurance = await DbContext.Set<ProfessionalInsurance>()
            .Include(i => i.Professional)
            .FirstOrDefaultAsync(i => i.Id == id
                && DbContext.ProfessionalWaterSuppliers.Any(pws => pws.ProfessionalId == i.ProfessionalId), cancellationToken)
            ?? throw new InvalidOperationException($"Insurance policy {id} not found for current water supplier.");

        insurance.InsuranceNumber = insuranceNumber;
        insurance.ExpirationDate = expirationDate;
        insurance.InsuranceCoverage = insuranceCoverage;

        await DbContext.SaveChangesAsync(cancellationToken);

        return insurance;
    }

    /// <summary>
    /// Insurance is held at company level, so a policy row carries no professional type of its own. The
    /// "Manage" button needs one anyway, to know which per-type details page to send staff to. This reads
    /// it off the professional's registration with the current water supplier (already tenant-scoped, see
    /// ScopedToWaterSupplier), preferring Bpat first since that is the program the page was built against.
    /// A professional registered for more than one program only ever gets the first match.
    /// </summary>
    public async Task<Dictionary<int, ProfessionalType?>> GetProfessionalTypesAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        var flagsByProfessionalId = await DbContext.ProfessionalWaterSuppliers
            .AsNoTracking()
            .Where(pws => professionalIds.Contains(pws.ProfessionalId))
            .ToDictionaryAsync(pws => pws.ProfessionalId, pws => pws, cancellationToken);

        return professionalIds.ToDictionary(id => id, id =>
        {
            if (!flagsByProfessionalId.TryGetValue(id, out var flags))
            {
                return (ProfessionalType?)null;
            }

            if (flags.HasBackflowTesting)
            {
                return ProfessionalType.Bpat;
            }

            if (flags.HasCsiInpection)
            {
                return ProfessionalType.CsiInspector;
            }

            if (flags.HasFogTransportation)
            {
                return ProfessionalType.FogTransporter;
            }

            if (flags.HasFogInspection)
            {
                return ProfessionalType.FogInspector;
            }

            return (ProfessionalType?)null;
        });
    }

    /// <summary>
    /// ProfessionalInsurance is an IProfessionalModel, not an ITenantModel, so no WaterSupplierId filter
    /// applies to it. ProfessionalWaterSupplier is a TenantModel, so joining through it borrows that
    /// tenant filter and limits the result to professionals registered with the current water supplier.
    /// </summary>
    private IQueryable<ProfessionalInsurance> ScopedToWaterSupplier()
    {
        return DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .Where(i => DbContext.ProfessionalWaterSuppliers.Any(pws => pws.ProfessionalId == i.ProfessionalId));
    }

    /// <summary>
    /// The three Insurance Management tabs. Buckets match the License Management ones so both pages
    /// answer "unverified / expired / expiring" the same way.
    /// </summary>
    private static IQueryable<ProfessionalInsurance> ApplyInsuranceFilter(IQueryable<ProfessionalInsurance> query, string? insuranceFilter)
    {
        var now = DateTime.UtcNow;
        var firstDayThisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

        return insuranceFilter switch
        {
            "unverified" => query.Where(i => i.ExpirationDate == null),
            "expired" => query.Where(i => i.ExpirationDate != null
                && i.ExpirationDate >= firstDayLastMonth
                && i.ExpirationDate < firstDayThisMonth),
            "expiring" => query.Where(i => i.ExpirationDate != null
                && i.ExpirationDate >= firstDayThisMonth
                && i.ExpirationDate < firstDayThisMonth.AddMonths(1)),
            _ => query
        };
    }
}