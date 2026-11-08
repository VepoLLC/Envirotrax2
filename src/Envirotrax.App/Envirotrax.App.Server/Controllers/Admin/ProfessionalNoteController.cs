using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

public abstract class ProfessionalNoteController : AdminBaseController
{
    private readonly IProfessionalNoteService _noteService;

    protected abstract ProfessionalType ProfessionalType { get; }

    protected ProfessionalNoteController(IProfessionalNoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var notes = await _noteService.GetAllByProfessionalAsync(professionalId, ProfessionalType, pageInfo, query, cancellationToken);

        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var added = await _noteService.AddForProfessionalAsync(professionalId, ProfessionalType, note, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{noteId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int noteId, [FromBody] ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var updated = await _noteService.UpdateForProfessionalAsync(professionalId, ProfessionalType, noteId, note, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{noteId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int noteId, CancellationToken cancellationToken)
    {
        var deleted = await _noteService.DeleteForProfessionalAsync(professionalId, ProfessionalType, noteId, cancellationToken);

        return deleted == null ? NotFound() : Ok();
    }
}
