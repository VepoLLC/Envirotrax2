using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalUserNoteRepository : IRepository<ProfessionalUserNote>
{
    Task<IEnumerable<ProfessionalUserNote>> GetAllByProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ProfessionalUserNote?> GetForProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, int noteId, CancellationToken cancellationToken);
}
