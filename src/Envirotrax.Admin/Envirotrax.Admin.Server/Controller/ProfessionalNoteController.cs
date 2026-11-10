using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers;

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
        var notes = await _noteService.GetAllAsync(ProfessionalType, professionalId, pageInfo, query, cancellationToken);

        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var added = await _noteService.AddAsync(ProfessionalType, professionalId, note, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{noteId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int noteId, [FromBody] ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        var updated = await _noteService.UpdateAsync(ProfessionalType, professionalId, noteId, note, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{noteId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int noteId, CancellationToken cancellationToken)
    {
        await _noteService.DeleteAsync(ProfessionalType, professionalId, noteId, cancellationToken);

        return Ok();
    }
}
