using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowOutOfServiceRequestRepository : Repository<BackflowOutOfServiceRequest>, IBackflowOutOfServiceRequestRepository
{
    public BackflowOutOfServiceRequestRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<BackflowOutOfServiceRequest> GetListQuery()
    {
        return base.GetListQuery()
            .Include(r => r.Test)
                .ThenInclude(t => t!.PropertyState)
            .Include(r => r.ReplacementAssemblyTest)
                .ThenInclude(t => t!.PropertyState)
            .Include(r => r.Bpat);
    }

    protected override IQueryable<BackflowOutOfServiceRequest> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(r => r.Test)
                .ThenInclude(t => t!.PropertyState)
            .Include(r => r.ReplacementAssemblyTest)
                .ThenInclude(t => t!.PropertyState)
            .Include(r => r.Bpat);
    }

    public override Task<IEnumerable<BackflowOutOfServiceRequest>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowOutOfServiceRequest.Id)] = SortOperator.Asc;
        }

        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<BackflowTest>> GetReplacementCandidatesAsync(int testId, CancellationToken cancellationToken)
    {
        var sourceTest = await DbContext.BackflowTests
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == testId, cancellationToken);

        if (sourceTest == null || !sourceTest.SiteId.HasValue)
        {
            return Enumerable.Empty<BackflowTest>();
        }

        // Mirrors V1: same site, current assemblies only, not already out of service, excluding the source test.
        var candidates = await DbContext.BackflowTests
            .AsNoTracking()
            .Include(t => t.PropertyState)
            .Where(t => t.SiteId == sourceTest.SiteId
                && t.WaterSupplierId == sourceTest.WaterSupplierId
                && t.Id != sourceTest.Id
                && t.IsCurrent
                && !t.OutOfService)
            .OrderByDescending(t => t.TestDate)
            .ToListAsync(cancellationToken);

        // V1 also excludes an assembly with the same serial number as the one being retired (same physical device).
        return candidates
            .Where(t => !SerialNumbersMatch(sourceTest.SerialNumber, t.SerialNumber))
            .ToList();
    }

    public Task<bool> HasRequestForTestAsync(int testId, CancellationToken cancellationToken)
    {
        return DbContext.BackflowOutOfServiceRequests
            .AsNoTracking()
            .AnyAsync(r => r.TestId == testId, cancellationToken);
    }

    public Task<int?> GetTestWaterSupplierIdAsync(int testId, CancellationToken cancellationToken)
    {
        return DbContext.BackflowTests
            .AsNoTracking()
            .Where(t => t.Id == testId)
            .Select(t => (int?)t.WaterSupplierId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<BackflowOutOfServiceRequest>> GetForWaterSupplierAsync(
        PageInfo pageInfo, Query query, OutOfServiceRequestStatusFilter status,
        OutOfServiceType? type, CancellationToken cancellationToken)
    {
        // Tenant scoping (WaterSupplierId == current) is applied automatically by TenantDbContext.
        var q = GetListQuery();

        switch (status)
        {
            case OutOfServiceRequestStatusFilter.AllUncleared:
                q = q.Where(r => r.OutOfServiceDate == null && r.ClearedDate == null);
                break;
            case OutOfServiceRequestStatusFilter.MarkedOutOfService:
                q = q.Where(r => r.OutOfServiceDate != null);
                break;
            case OutOfServiceRequestStatusFilter.Cleared:
                q = q.Where(r => r.ClearedDate != null);
                break;
            case OutOfServiceRequestStatusFilter.All:
            default:
                break;
        }

        if (type.HasValue)
        {
            q = q.Where(r => r.Type == type.Value);
        }

        var paginated = await q
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<bool> ClearAsync(int id, CancellationToken cancellationToken)
    {
        // Tenant-scoped, so only the supplier's own rows are visible.
        var request = await DbContext.BackflowOutOfServiceRequests
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (request == null)
        {
            return false;
        }

        request.ClearedDate = DateTime.UtcNow;

        await DbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static bool SerialNumbersMatch(string? first, string? second)
    {
        var a = NormalizeSerialNumber(first);
        var b = NormalizeSerialNumber(second);

        return a.Length > 0 && a == b;
    }

    private static string NormalizeSerialNumber(string? serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            return string.Empty;
        }

        return new string(serialNumber.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
    }
}
