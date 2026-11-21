using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

public abstract class ProfessionalUserNoteController : AdminBaseController
{
    private readonly IProfessionalNoteService _noteService;

    protected abstract ProfessionalType ProfessionalType { get; }

    protected ProfessionalUserNoteController(IProfessionalNoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, int userId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var notes = await _noteService.GetAllByProfessionalUserAsync(professionalId, userId, ProfessionalType, pageInfo, query, cancellationToken);

        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, int userId, [FromBody] ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var added = await _noteService.AddForProfessionalUserAsync(professionalId, userId, ProfessionalType, note, cancellationToken);

        return added == null ? NotFound() : Ok(added);
    }

    [HttpPut("{noteId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int userId, int noteId, [FromBody] ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var updated = await _noteService.UpdateForProfessionalUserAsync(professionalId, userId, ProfessionalType, noteId, note, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{noteId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int userId, int noteId, CancellationToken cancellationToken)
    {
        var deleted = await _noteService.DeleteForProfessionalUserAsync(professionalId, userId, ProfessionalType, noteId, cancellationToken);

        return deleted == null ? NotFound() : Ok();
    }
}
