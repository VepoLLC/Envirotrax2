using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowTestRepository : IRepository<BackflowTest>
{
    Task<IEnumerable<BackflowTest>> GetAllWithFacilityTypesAsync(PageInfo pageInfo, Query query, List<FacilityType> facilityTypes, CancellationToken cancellationToken);
}
