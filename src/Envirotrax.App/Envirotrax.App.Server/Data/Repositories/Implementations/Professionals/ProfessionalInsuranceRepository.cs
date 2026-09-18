
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
    /// IgnoreQueryFilters is load-bearing: under ProfessionalDbContext the ProfessionalId filter would
    /// rewrite this into "the caller's policies", breaking the sub-account lookup of its master's policies.
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
        var baseQuery = ApplyInsuranceFilter(ScopedInsurances().Include(i => i.Professional), insuranceFilter);

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
        return await ApplyInsuranceFilter(ScopedInsurances(), insuranceFilter).CountAsync(cancellationToken);
    }

    public async Task<ProfessionalInsurance> GetForWaterSupplierAsync(int id, CancellationToken cancellationToken)
    {
        return await ScopedInsurances()
            .Include(i => i.Professional)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken)
            ?? throw new InvalidOperationException($"Insurance policy {id} not found for current water supplier.");
    }

    public async Task<ProfessionalInsurance> UpdateForWaterSupplierAsync(int id, string insuranceNumber, DateTime? expirationDate, decimal? insuranceCoverage, CancellationToken cancellationToken)
    {
        var insurance = await DbContext.Set<ProfessionalInsurance>()
            .ScopedToWaterSupplier(DbContext)
            .Include(i => i.Professional)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken)
            ?? throw new InvalidOperationException($"Insurance policy {id} not found for current water supplier.");

        insurance.InsuranceNumber = insuranceNumber;
        insurance.ExpirationDate = expirationDate;
        insurance.InsuranceCoverage = insuranceCoverage;

        await DbContext.SaveChangesAsync(cancellationToken);

        return insurance;
    }

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

    private IQueryable<ProfessionalInsurance> ScopedInsurances()
    {
        return DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .ScopedToWaterSupplier(DbContext);
    }

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