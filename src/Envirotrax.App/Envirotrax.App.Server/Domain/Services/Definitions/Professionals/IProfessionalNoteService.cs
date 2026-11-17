using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalNoteService : IService<ProfessionalNoteDto>
{
    Task<IPagedData<ProfessionalNoteDto>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ProfessionalNoteDto> AddForProfessionalAsync(int professionalId, ProfessionalType professionalType, ProfessionalNoteDto note, CancellationToken cancellationToken);
    Task<ProfessionalNoteDto?> UpdateForProfessionalAsync(int professionalId, ProfessionalType professionalType, int noteId, ProfessionalNoteDto note, CancellationToken cancellationToken);
    Task<ProfessionalNoteDto?> DeleteForProfessionalAsync(int professionalId, ProfessionalType professionalType, int noteId, CancellationToken cancellationToken);

    // A user's own comment log (ProfessionalUserNotes). Add returns null when the user is not one of the company's,
    // update and delete when the note is not that user's.
    Task<IPagedData<ProfessionalNoteDto>> GetAllByProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ProfessionalNoteDto?> AddForProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, ProfessionalNoteDto note, CancellationToken cancellationToken);
    Task<ProfessionalNoteDto?> UpdateForProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, int noteId, ProfessionalNoteDto note, CancellationToken cancellationToken);
    Task<ProfessionalNoteDto?> DeleteForProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, int noteId, CancellationToken cancellationToken);
}
