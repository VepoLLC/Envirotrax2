
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals")]
public class ProfessionalControler : ProfessionalProtectedController
{
    private readonly IProfessionalService _professionalService;

    public ProfessionalControler(IProfessionalService professionalService)
    {
        _professionalService = professionalService;
    }

    [HttpPut("my")]
    [Authorize(Roles = RoleDefinitions.Professionals.Admin)]
    public async Task<IActionResult> UpdateMyAsync(ProfessionalDto professional)
    {
        var updated = await _professionalService.UpdateAsync(professional);
        return Ok(updated);
    }
}

// We this controller because /api/professionals/my endpoint requires the user to be logged in, but it doesn't require having Professional role.
// When that endpoint is called, the professional role is not set yet. That is why we need a different authroize attribute.
[Authorize]
[ApiController]
[Route("api/professionals")]
public class MyProfessionalsController : ControllerBase
{
    private readonly IProfessionalService _professionalService;

    public MyProfessionalsController(IProfessionalService professionalService)
    {
        _professionalService = professionalService;
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

    [HttpGet("my")]
    public async Task<IActionResult> GetAllMyAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var professionals = await _professionalService.GetAllMyAsync(pageInfo, query, cancellationToken);
        return Ok(professionals);
    }

    [HttpPost("my")]
    public async Task<IActionResult> AddMyAsync(CreateProfessionalDto createProfessional)
    {
        var added = await _professionalService.AddMyAsync(createProfessional);
        return Ok(added);
    }
}