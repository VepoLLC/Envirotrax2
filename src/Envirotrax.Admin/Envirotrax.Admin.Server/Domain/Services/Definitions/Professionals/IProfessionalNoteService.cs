using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalNoteService
{
    Task<IPagedData<ProfessionalNoteDto>> GetAllAsync(ProfessionalType professionalType, int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalNoteDto?> AddAsync(ProfessionalType professionalType, int professionalId, ProfessionalNoteDto note, CancellationToken cancellationToken);

    Task<ProfessionalNoteDto?> UpdateAsync(ProfessionalType professionalType, int professionalId, int noteId, ProfessionalNoteDto note, CancellationToken cancellationToken);

    Task DeleteAsync(ProfessionalType professionalType, int professionalId, int noteId, CancellationToken cancellationToken);
}
