using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalNoteService
{
    // With a userId, these work on that user's own comment log (ProfessionalUserNotes) instead of the company's.
    Task<IPagedData<ProfessionalNoteDto>> GetAllAsync(ProfessionalType professionalType, int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, int? userId = null);

    Task<ProfessionalNoteDto?> AddAsync(ProfessionalType professionalType, int professionalId, ProfessionalNoteDto note, CancellationToken cancellationToken, int? userId = null);

    Task<ProfessionalNoteDto?> UpdateAsync(ProfessionalType professionalType, int professionalId, int noteId, ProfessionalNoteDto note, CancellationToken cancellationToken, int? userId = null);

    Task DeleteAsync(ProfessionalType professionalType, int professionalId, int noteId, CancellationToken cancellationToken, int? userId = null);
}
