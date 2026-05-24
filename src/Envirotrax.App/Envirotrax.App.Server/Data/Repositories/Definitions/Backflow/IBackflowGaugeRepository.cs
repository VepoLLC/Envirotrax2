
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowGaugeRepository : IRepository<BackflowGauge>
{
    Task<IEnumerable<BackflowGauge>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
