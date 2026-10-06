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

    public Task<SiteSchedule?> GetMyAsync(int siteId, int userId, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        return Entity
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.SiteId == siteId && s.UserId == userId && s.ProfessionalType == professionalType, cancellationToken);
    }

    public async Task<IEnumerable<SiteSchedule>> GetMyBySiteIdsAsync(IEnumerable<int> siteIds, int userId, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        return await Entity
            .AsNoTracking()
            .Where(s => siteIds.Contains(s.SiteId) && s.UserId == userId && s.ProfessionalType == professionalType)
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
        var deletedCount = await Entity
            .Where(s => s.SiteId == siteId && s.UserId == userId && s.ProfessionalType == professionalType)
            .ExecuteDeleteAsync();

        return deletedCount > 0;
    }
}
