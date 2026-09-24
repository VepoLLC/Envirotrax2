using Envirotrax.App.Server.Data.Models.Logs;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.DbContexts;

public partial class TenantDbContext
{
    // Set when a record comes in from an external submission; it is bookkeeping, not a user edit.
    private const string SubmissionIdPropertyName = "SubmissionId";

    private readonly List<RecordLog> _stagedLogs = new();

    private bool _logDataForCurrentSave;

    /// <summary>
    /// Saves, and writes a RecordLog entry for every changed entity marked with
    /// <see cref="RecordLoggedAttribute"/> that actually has field changes to report.
    /// </summary>
    /// <remarks>
    /// The log rows join the same SaveChanges as the rows they describe, so an edit and its log are
    /// written in one transaction — neither can be committed without the other.
    ///
    /// This is the baseline: it records the raw field diff. Call
    /// <c>IRecordLogService.AddAsync</c> instead wherever the log needs a hand-written message
    /// (record added, record deleted, or a change the diff cannot express).
    /// </remarks>
    public async Task<int> SaveChangesAndLogAsync(CancellationToken cancellationToken = default)
    {
        _logDataForCurrentSave = true;

        try
        {
            // Awaited rather than returned: the finally must not clear the flag before the save runs.
            return await SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // The edit did not commit, so its log rows must not stay staged in the change tracker:
            // the context is scoped to the request, and any later save on it would insert them.
            DiscardStagedLogs();
            throw;
        }
        finally
        {
            _logDataForCurrentSave = false;
            _stagedLogs.Clear();
        }
    }

    private void DiscardStagedLogs()
    {
        foreach (var log in _stagedLogs)
        {
            Entry(log).State = EntityState.Detached;
        }
    }

    protected override void OnSavingChanges()
    {
        base.OnSavingChanges();

        if (!_logDataForCurrentSave)
        {
            return;
        }

        // Materialized because AddRange below modifies the change tracker.
        var entries = ChangeTracker.Entries().ToList();
        var logs = new List<RecordLog>();

        foreach (var entry in entries)
        {
            if (entry.State != EntityState.Modified || entry.Entity is RecordLog)
            {
                continue;
            }

            var descriptor = RecordLogDescriptor.Resolve(entry.Metadata);

            if (descriptor == null)
            {
                continue;
            }

            var description = BuildChangeDescription(entry);

            // Nothing actually changed — same guard the hand-written call sites apply.
            if (description.Length == 0)
            {
                continue;
            }

            logs.Add(descriptor.CreateLog(entry, _tenantProvider, RecordLogType.Edit, description));
        }

        RecordLogs.AddRange(logs);
        _stagedLogs.AddRange(logs);
    }

    protected override bool IsChangeDescriptionSkipped(string propertyName)
    {
        return base.IsChangeDescriptionSkipped(propertyName)
            || propertyName == SubmissionIdPropertyName;
    }
}
