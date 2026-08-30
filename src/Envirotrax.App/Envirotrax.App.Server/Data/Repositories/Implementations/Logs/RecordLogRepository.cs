using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Logs;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Logs;

public class RecordLogRepository : Repository<RecordLog, long>, IRecordLogRepository
{
    public RecordLogRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<RecordLog> GetListQuery()
    {
        return base.GetListQuery()
            .Include(log => log.CreatedBy);
    }

    public async Task<List<RecordLog>> GetByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken)
    {
        return await GetListQuery()
            .Where(log => log.TableName == tableName && log.RecordId == recordId)
            .OrderByDescending(log => log.CreatedTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken)
    {
        return await GetListQuery()
            .CountAsync(log => log.TableName == tableName && log.RecordId == recordId, cancellationToken);
    }
}
