
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowGaugeRepository : IRepository<BackflowGauge>
{
    Task<IEnumerable<BackflowGauge>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<BackflowGauge?> UpdateGaugeAsync(BackflowGauge model);

    Task<IEnumerable<BackflowGauge>> GetUnverifiedByWaterSupplierAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<int> GetUnverifiedCountByWaterSupplierAsync(CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalUser>> GetProfessionalUsersAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);
}
