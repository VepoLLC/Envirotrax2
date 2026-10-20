
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// Admin BPAT search and account details. A route shell only - it reuses the existing IBackflowTesterService.SearchAsync that the
/// water-supplier window uses. That endpoint (api/backflow/testers/search) is unreachable by an admin caller
/// (different scope, role and feature gate), so the admin surface needs its own route, but not its own search.
/// IDbContextSelector already routes admin requests to AdminDbContext, which disables tenant filtering.
/// </summary>
[Route("api/admin/backflow/testers")]
public class BackflowTesterController : AdminBaseController
{
    private readonly IBackflowTesterService _testerService;
    private readonly IProfessionalService _professionalService;

    public BackflowTesterController(IBackflowTesterService testerService, IProfessionalService professionalService)
    {
        _testerService = testerService;
        _professionalService = professionalService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] BackflowTesterSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var testers = await _testerService.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(testers);
    }

    [HttpGet("{professionalId}")]
    public async Task<IActionResult> GetAsync(int professionalId, CancellationToken cancellationToken)
    {
        var professional = await _professionalService.GetAsync(professionalId, cancellationToken);

        return professional == null ? NotFound() : Ok(professional);
    }

    // The account is saved through IProfessionalService, the same as the admin CSI window, because its repository
    // keeps AccountBalance and HasWiseGuys out of the update. IBackflowTesterService's repository does not, so
    // saving through it would overwrite the balance with the value the mapping leaves at zero.
    [HttpPut("{professionalId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, [FromBody] ProfessionalDto professional, CancellationToken cancellationToken)
    {
        if (!await _professionalService.ExistsAsync(professionalId, cancellationToken))
        {
            return NotFound();
        }

        professional.Id = professionalId;

        var updated = await _professionalService.UpdateAsync(professional);

        return Ok(updated);
    }
}
