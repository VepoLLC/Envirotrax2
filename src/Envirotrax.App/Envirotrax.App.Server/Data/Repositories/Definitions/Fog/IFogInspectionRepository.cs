using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogInspectionRepository : IRepository<FogInspection>
{
    Task<IEnumerable<FogInspection>> SearchForProfessionalAsync(
        int professionalId, PageInfo pageInfo, Query query,
        List<FacilityType> facilityTypes, bool latestOnly, CancellationToken cancellationToken);
}
