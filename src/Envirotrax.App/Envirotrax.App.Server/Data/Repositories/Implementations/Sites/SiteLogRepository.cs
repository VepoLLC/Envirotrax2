using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public class SiteLogRepository : Repository<SiteLog>, ISiteLogRepository
{
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public SiteLogRepository(IDbContextSelector dbContextSelector, ITimeZoneHelperService timeZoneHelper)
        : base(dbContextSelector)
    {
        _timeZoneHelper = timeZoneHelper;
    }

    protected override IQueryable<SiteLog> GetListQuery()
    {
        return base.GetListQuery()
            .Include(sl => sl.CreatedBy)
            .Include(sl => sl.Assembly)
            .Include(sl => sl.Site).ThenInclude(s => s!.State)
            .Include(sl => sl.Site).ThenInclude(s => s!.MailingState);
    }

    protected override IQueryable<SiteLog> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(sl => sl.CreatedBy)
            .Include(sl => sl.Assembly);
    }

    public override Task<IEnumerable<SiteLog>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(SiteLog.Id)] = SortOperator.Desc;
        }
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<SiteLog>> GetForManagementAsync(PageInfo pageInfo, Query query, string? logTypeFilter, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(SiteLog.Id)] = SortOperator.Desc;
        }

        var paginated = await ApplyLogTypeFilter(GetListQuery(), logTypeFilter)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    private IQueryable<SiteLog> ApplyLogTypeFilter(IQueryable<SiteLog> query, string? logTypeFilter)
    {
        var now = _timeZoneHelper.GetUserLocalTime();
        var in30Days = now.AddDays(30);

        return logTypeFilter switch
        {
            "expired" => query.Where(sl => sl.LogType == SiteLogType.Reminder && sl.ReviewDate < now),
            "expiring" => query.Where(sl => sl.LogType == SiteLogType.Reminder && sl.ReviewDate >= now && sl.ReviewDate <= in30Days),
            _ => query
        };
    }

    public async Task<IEnumerable<SiteLog>> GetBySiteAsync(int siteId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(SiteLog.Id)] = SortOperator.Desc;
        }

        var paginated = await GetListQuery()
            .Where(sl => sl.SiteId == siteId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<int> CountBySiteAsync(int siteId, CancellationToken cancellationToken)
    {
        return Entity.CountAsync(sl => sl.SiteId == siteId, cancellationToken);
    }

    public async Task<IEnumerable<SiteLog>> GetBySiteIdsAsync(IEnumerable<int> siteIds, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(-365);

        return await GetListQuery()
            .Where(sl => siteIds.Contains(sl.SiteId) && sl.CreatedTime > cutoff)
            .OrderByDescending(sl => sl.Id)
            .ToListAsync(cancellationToken);
    }
}
