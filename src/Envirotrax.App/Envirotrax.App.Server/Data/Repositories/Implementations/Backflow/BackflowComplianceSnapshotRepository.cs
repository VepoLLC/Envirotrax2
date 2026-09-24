using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

// Id-less composite-key model (WaterSupplierId, ReportDate), so this follows the custom-repository
// pattern (RolePermission / UserRole) rather than the generic int-key Repository<TModel> base. All
// queries run through the tenant query filter, so ReportDate alone uniquely identifies the current
// supplier's row, and WaterSupplierId is stamped by the context on save.
public class BackflowComplianceSnapshotRepository : IBackflowComplianceSnapshotRepository
{
    private readonly TenantDbContext _dbContext;

    public BackflowComplianceSnapshotRepository(IDbContextSelector dbContextSelector)
    {
        _dbContext = dbContextSelector.Current;
    }

    public async Task<IEnumerable<BackflowComplianceSnapshot>> GetMonthlyHistoryAsync(CancellationToken cancellationToken)
    {
        return await _dbContext
            .BackflowComplianceSnapshots
            .AsNoTracking()
            .Where(s => s.ReportDate.Day == 1)
            .OrderBy(s => s.ReportDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<BackflowComplianceSnapshot?> GetLatestAsync(CancellationToken cancellationToken)
    {
        return await _dbContext
            .BackflowComplianceSnapshots
            .AsNoTracking()
            .OrderByDescending(s => s.ReportDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BackflowComplianceSnapshot> UpsertAsync(BackflowComplianceSnapshot snapshot, CancellationToken cancellationToken)
    {
        var existing = await _dbContext
            .BackflowComplianceSnapshots
            .SingleOrDefaultAsync(s => s.ReportDate == snapshot.ReportDate, cancellationToken);

        if (existing == null)
        {
            _dbContext.BackflowComplianceSnapshots.Add(snapshot);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return snapshot;
        }

        existing.Total = snapshot.Total;
        existing.Compliant = snapshot.Compliant;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return existing;
    }
}
