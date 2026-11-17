using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common;
using Envirotrax.Common.Data.Models;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalNoteService : Service<ProfessionalNote, ProfessionalNoteDto>, IProfessionalNoteService
{
    private readonly IProfessionalNoteRepository _noteRepository;
    private readonly IProfessionalUserNoteRepository _userNoteRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IWaterSupplierRepository _waterSupplierRepository;
    private readonly IAuthService _authService;

    private bool IsAdmin => _authService.HasScope(ScopeDefinitions.AdminInternal)
        || _authService.HasAnyRole(RoleDefinitions.SuperUser);

    public ProfessionalNoteService(
        IMapper mapper,
        IProfessionalNoteRepository repository,
        IProfessionalUserNoteRepository userNoteRepository,
        IProfessionalUserRepository professionalUserRepository,
        IWaterSupplierRepository waterSupplierRepository,
        IAuthService authService)
        : base(mapper, repository)
    {
        _noteRepository = repository;
        _userNoteRepository = userNoteRepository;
        _professionalUserRepository = professionalUserRepository;
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

        await KeepAdminNoteUnderAdminSupplierAsync(model, cancellationToken);

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

    public async Task<IPagedData<ProfessionalNoteDto>> GetAllByProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalUserNote, ProfessionalNoteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalUserNote, ProfessionalNoteDto>(Mapper);

        var notes = await _userNoteRepository.GetAllByProfessionalUserAsync(professionalId, userId, professionalType, pageInfo, query, cancellationToken);

        return notes.Select(note => Mapper.Map<ProfessionalNoteDto>(note)).ToPagedData(pageInfo);
    }

    public async Task<ProfessionalNoteDto?> AddForProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        if (!await _professionalUserRepository.ExistsForProfessionalAsync(professionalId, userId, cancellationToken))
        {
            return null;
        }

        var model = Mapper.Map<ProfessionalUserNote>(note);

        model.Id = 0;
        model.ProfessionalId = professionalId;
        model.UserId = userId;
        model.ProfessionalType = professionalType;

        await KeepAdminNoteUnderAdminSupplierAsync(model, cancellationToken);

        var added = await _userNoteRepository.AddAsync(model);

        return Mapper.Map<ProfessionalNoteDto>(added);
    }

    public async Task<ProfessionalNoteDto?> UpdateForProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, int noteId, ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var existing = await _userNoteRepository.GetForProfessionalUserAsync(professionalId, userId, professionalType, noteId, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        existing.NoteText = note.NoteText;

        var updated = await _userNoteRepository.UpdateAsync(existing);

        return Mapper.Map<ProfessionalNoteDto?>(updated);
    }

    public async Task<ProfessionalNoteDto?> DeleteForProfessionalUserAsync(int professionalId, int userId, ProfessionalType professionalType, int noteId, CancellationToken cancellationToken)
    {
        var existing = await _userNoteRepository.GetForProfessionalUserAsync(professionalId, userId, professionalType, noteId, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        var deleted = await _userNoteRepository.DeleteAsync(noteId);

        return Mapper.Map<ProfessionalNoteDto?>(deleted);
    }

    // As in V1, a note an admin writes, in the Admin portal or as a super user in the App, is seen only by
    // admins, so it is kept under the Vepo LLC supplier rather than the supplier being worked in.
    private async Task KeepAdminNoteUnderAdminSupplierAsync(TenantModel<WaterSupplier> note, CancellationToken cancellationToken)
    {
        if (IsAdmin)
        {
            note.WaterSupplierId = await _waterSupplierRepository.GetEnvirotraxAdminSupplierIdAsync(cancellationToken);
        }
    }
}
