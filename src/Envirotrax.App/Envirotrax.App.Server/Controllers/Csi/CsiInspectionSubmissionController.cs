using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi;

[Route("api/csi/inspections/submission")]
public class CsiInspectionSubmissionController : ProfessionalProtectedController
{
    private readonly ICsiSubmissionService _submissionService;

    public CsiInspectionSubmissionController(ICsiSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpGet("create/{siteId:int}")]
    public async Task<IActionResult> GetCreateViewModelAsync(int siteId, CancellationToken cancellationToken)
    {
        var viewModel = await _submissionService.GetCreateViewModelAsync(siteId, cancellationToken);

        if (viewModel == null)
        {
            return NotFound();

        }
        
        return Ok(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAsync(CsiSubmissionSaveRequest request, CancellationToken cancellationToken)
    {
        var result = await _submissionService.SubmitAsync(request, cancellationToken);
        return Ok(result);
    }
}
