using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public class SiteScheduleRepository : Repository<SiteSchedule>, ISiteScheduleRepository
{
    public SiteScheduleRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<SiteSchedule>> GetMyBySiteIdsAsync(IEnumerable<int> siteIds, int userId, CancellationToken cancellationToken)
    {
        return await Entity
            .AsNoTracking()
            .Where(s => siteIds.Contains(s.SiteId) && s.UserId == userId)
            .OrderBy(s => s.ScheduleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<SiteSchedule> SetAsync(SiteSchedule schedule)
    {
        var existing = await Entity.SingleOrDefaultAsync(s =>
            s.SiteId == schedule.SiteId
            && s.UserId == schedule.UserId
            && s.ProfessionalType == schedule.ProfessionalType);

        if (existing == null)
        {
            return await AddAsync(schedule);
        }

        existing.ScheduleDate = schedule.ScheduleDate;

        await DbContext.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> ClearAsync(int siteId, int userId, ProfessionalType professionalType)
    {
        var schedules = await Entity
            .Where(s => s.SiteId == siteId && s.UserId == userId && s.ProfessionalType == professionalType)
            .ToListAsync();

        Entity.RemoveRange(schedules);

        return await DbContext.SaveChangesAsync() > 0;
    }
}
