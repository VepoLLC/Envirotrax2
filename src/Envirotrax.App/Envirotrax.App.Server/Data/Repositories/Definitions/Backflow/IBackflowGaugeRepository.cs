
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowGaugeRepository : IRepository<BackflowGauge>
{
    Task<IEnumerable<BackflowGauge>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<BackflowGauge>> GetAllByProfessionalUserAsync(int professionalId, int userId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<BackflowGauge?> UpdateGaugeAsync(BackflowGauge model);

    Task<int> CountCompanyAndUserGaugesAsync(int userId, CancellationToken cancellationToken);
    Task<IEnumerable<BackflowGauge>> GetCompanyAndUserGaugesAsync(int userId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<BackflowGauge?> GetCompanyOrUserGaugeAsync(int gaugeId, int userId, CancellationToken cancellationToken);

    Task<IEnumerable<BackflowGauge>> GetUnverifiedAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<int> CountUnverifiedAsync(CancellationToken cancellationToken);

    Task<IEnumerable<ProfessionalUser>> GetProfessionalUsersAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);
}
