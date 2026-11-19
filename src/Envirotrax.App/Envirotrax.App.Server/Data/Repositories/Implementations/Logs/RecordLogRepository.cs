using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Logs;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Logs;

public class RecordLogRepository : Repository<RecordLog, long>, IRecordLogRepository
{
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public RecordLogRepository(IDbContextSelector dbContextSelector, ITimeZoneHelperService timeZoneHelper)
        : base(dbContextSelector)
    {
        _timeZoneHelper = timeZoneHelper;
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

    public override Task<IEnumerable<RecordLog>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(RecordLog.Id)] = SortOperator.Desc;
        }

        CreatedTimeDaySearch.Apply(query, _timeZoneHelper.GetUserTimeZone());

        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }
}
