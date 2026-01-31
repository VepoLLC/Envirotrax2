
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals")]
public class ProfessionalControler : ProtectedController
{
    private readonly IProfessionalService _professionalService;

    public ProfessionalControler(IProfessionalService professionalService)
    {
        _professionalService = professionalService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetAllMyAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var professionals = await _professionalService.GetAllMyAsync(pageInfo, query, cancellationToken);
        return Ok(professionals);
    }

    [HttpGet("my/current")]
    public async Task<IActionResult> GetLoggedInProfessionalAsync(CancellationToken cancellationToken)
    {
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);

        if (professional != null)
        {
            return Ok(professional);
        }

        return NotFound();
    }

    [HttpPost("my")]
    public async Task<IActionResult> AddMyAsync(ProfessionalDto professional)
    {
        var added = await _professionalService.AddAsync(professional);
        return Ok(added);
    }
}