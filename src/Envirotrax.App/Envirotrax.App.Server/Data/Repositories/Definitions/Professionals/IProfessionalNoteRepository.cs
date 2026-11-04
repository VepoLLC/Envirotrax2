using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalNoteRepository : IRepository<ProfessionalNote>
{
    Task<IEnumerable<ProfessionalNote>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ProfessionalNote?> GetForProfessionalAsync(int professionalId, ProfessionalType professionalType, int noteId, CancellationToken cancellationToken);
}
