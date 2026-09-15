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

    public async Task<IEnumerable<BackflowComplianceSnapshot>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext
            .BackflowComplianceSnapshots
            .AsNoTracking()
            .OrderBy(s => s.ReportDate)
            .ToListAsync(cancellationToken);
    }

    // Add the month's snapshot if it doesn't exist yet, otherwise refresh its counts. Used by the
    // monthly job to write a single supplier/month row.
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

    // Upsert many months in one read + one write pass. Only the existing rows whose ReportDate is in
    // the incoming set are loaded (SQL IN), so a partial refresh never pulls the supplier's full
    // history. Existing rows are updated in place; missing ones are inserted together.
    public async Task BulkUpsertAsync(IEnumerable<BackflowComplianceSnapshot> snapshots, CancellationToken cancellationToken)
    {
        var incoming = snapshots.ToList();

        var reportDates = incoming
            .Select(s => s.ReportDate)
            .ToList();

        var existingByDate = await _dbContext
            .BackflowComplianceSnapshots
            .Where(s => reportDates.Contains(s.ReportDate))
            .ToDictionaryAsync(s => s.ReportDate, cancellationToken);

        var toAdd = new List<BackflowComplianceSnapshot>();

        foreach (var snapshot in incoming)
        {
            if (existingByDate.TryGetValue(snapshot.ReportDate, out var existing))
            {
                existing.Total = snapshot.Total;
                existing.Compliant = snapshot.Compliant;
            }
            else
            {
                toAdd.Add(snapshot);
            }
        }

        _dbContext.BackflowComplianceSnapshots.AddRange(toAdd);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
