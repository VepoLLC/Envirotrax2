using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowTestRepository : IRepository<BackflowTest>
{
    Task<IEnumerable<BackflowTest>> GetAllAsync(PageInfo pageInfo, Query query, int? gisAreaId, CancellationToken cancellationToken);
}
