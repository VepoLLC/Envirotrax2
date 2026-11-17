using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Logs;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Logs;
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

    public async Task<IEnumerable<RecordLog>> SearchAsync(RecordLogSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(RecordLog.Id)] = SortOperator.Desc;
        }

        CreatedTimeDaySearch.Apply(query, _timeZoneHelper.GetUserTimeZone());

        var dbQuery = GetListQuery()
            .Where(query.Filter)
            .Where(log => log.TableName == criteria.TableName)
            .WhereIf(criteria.WaterSupplierId.HasValue, log => log.WaterSupplierId == criteria.WaterSupplierId)
            .WhereIf(criteria.RecordId.HasValue, log => log.RecordId == criteria.RecordId)
            .WhereIf(!string.IsNullOrWhiteSpace(criteria.UserEmail), log => log.CreatedBy!.Email == criteria.UserEmail)
            .WhereIf(criteria.LogType.HasValue, log => log.LogType == criteria.LogType);

        var paginated = await dbQuery
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
