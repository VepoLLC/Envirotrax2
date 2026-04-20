
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalUserRepository : IRepository<ProfessionalUser>
{
    Task<ProfessionalUser?> UpdateNonSensitiveDataAsync(ProfessionalUser user);
    Task<IEnumerable<ProfessionalUser>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalUser>> GetCsiInspectorsForProfessionalAsync(int professionalId, CancellationToken cancellationToken);
}