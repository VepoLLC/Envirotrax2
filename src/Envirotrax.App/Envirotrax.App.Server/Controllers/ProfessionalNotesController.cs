using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers;

public abstract class ProfessionalNotesController : WaterSupplierProtectedController
{
    private readonly IProfessionalNoteService _noteService;

    protected abstract ProfessionalType ProfessionalType { get; }

    protected ProfessionalNotesController(IProfessionalNoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var notes = await _noteService.GetAllByProfessionalAsync(professionalId, ProfessionalType, pageInfo, query, cancellationToken);

        return Ok(notes);
    }

    [HttpPost]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalNoteDto note)
    {
        var added = await _noteService.AddForProfessionalAsync(professionalId, ProfessionalType, note);

        return Ok(added);
    }

    [HttpPut("{noteId}")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateAsync(int professionalId, int noteId, [FromBody] ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var updated = await _noteService.UpdateForProfessionalAsync(professionalId, ProfessionalType, noteId, note, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{noteId}")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> DeleteAsync(int professionalId, int noteId, CancellationToken cancellationToken)
    {
        var deleted = await _noteService.DeleteForProfessionalAsync(professionalId, ProfessionalType, noteId, cancellationToken);

        return deleted == null ? NotFound() : Ok();
    }
}
