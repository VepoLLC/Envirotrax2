using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Sites;

public interface ISiteScheduleRepository : IRepository<SiteSchedule>
{
    Task<SiteSchedule?> GetMyAsync(int siteId, int userId, ProfessionalType professionalType, CancellationToken cancellationToken);
    Task<IEnumerable<SiteSchedule>> GetMyBySiteIdsAsync(IEnumerable<int> siteIds, int userId, ProfessionalType professionalType, CancellationToken cancellationToken);
    Task<SiteSchedule> SetAsync(SiteSchedule schedule);
    Task<bool> ClearAsync(int siteId, int userId, ProfessionalType professionalType);
}
