using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Sites;

public interface ISiteScheduleService : IService<SiteScheduleDto>
{
    Task<IEnumerable<SiteScheduleDto>> GetMyBySiteIdsAsync(IEnumerable<int> siteIds, CancellationToken cancellationToken);
    Task<SiteScheduleDto?> SetMyAsync(int siteId, SiteScheduleDto dto, CancellationToken cancellationToken);
    Task<bool> ClearMyAsync(int siteId, ProfessionalType professionalType);
}
