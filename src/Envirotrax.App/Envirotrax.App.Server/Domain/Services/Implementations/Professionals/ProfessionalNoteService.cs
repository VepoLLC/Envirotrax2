using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalNoteService : Service<ProfessionalNote, ProfessionalNoteDto>, IProfessionalNoteService
{
    private readonly IProfessionalNoteRepository _noteRepository;
    private readonly IWaterSupplierRepository _waterSupplierRepository;
    private readonly IAuthService _authService;

    private bool IsAdmin => _authService.HasScope(ScopeDefinitions.AdminInternal);

    public ProfessionalNoteService(IMapper mapper, IProfessionalNoteRepository repository, IWaterSupplierRepository waterSupplierRepository, IAuthService authService)
        : base(mapper, repository)
    {
        _noteRepository = repository;
        _waterSupplierRepository = waterSupplierRepository;
        _authService = authService;
    }

    public async Task<IPagedData<ProfessionalNoteDto>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalNote, ProfessionalNoteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalNote, ProfessionalNoteDto>(Mapper);

        var notes = await _noteRepository.GetAllByProfessionalAsync(professionalId, professionalType, pageInfo, query, cancellationToken);

        return notes.Select(note => MapToDto(note)!).ToPagedData(pageInfo);
    }

    public async Task<ProfessionalNoteDto> AddForProfessionalAsync(int professionalId, ProfessionalType professionalType, ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var model = MapToModel(note)!;

        model.Id = 0;
        model.ProfessionalId = professionalId;
        model.ProfessionalType = professionalType;

        // The admin context does not stamp a tenant, so admin notes are kept under the Vepo LLC supplier,
        // where no other water supplier can see them.
        if (IsAdmin)
        {
            model.WaterSupplierId = await _waterSupplierRepository.GetEnvirotraxAdminSupplierIdAsync(cancellationToken);
        }

        var added = await _noteRepository.AddAsync(model);

        return MapToDto(added)!;
    }

    public async Task<ProfessionalNoteDto?> UpdateForProfessionalAsync(int professionalId, ProfessionalType professionalType, int noteId, ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var existing = await _noteRepository.GetForProfessionalAsync(professionalId, professionalType, noteId, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        existing.NoteText = note.NoteText;

        var updated = await _noteRepository.UpdateAsync(existing);

        return MapToDto(updated);
    }

    public async Task<ProfessionalNoteDto?> DeleteForProfessionalAsync(int professionalId, ProfessionalType professionalType, int noteId, CancellationToken cancellationToken)
    {
        var existing = await _noteRepository.GetForProfessionalAsync(professionalId, professionalType, noteId, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        return await DeleteAsync(noteId);
    }
}
