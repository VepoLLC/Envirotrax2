using Envirotrax.App.Server.Domain.Services.Definitions.Letters;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Letters;

[Route("api/letter-history")]
[HasFeature(FeatureType.BackflowTesting, FeatureType.CsiInspection)]
[PermissionResource(PermissionType.AccountInformation)]
public class LetterHistoryController : WaterSupplierProtectedController
{
    private readonly ILetterHistoryService _letterHistoryService;

    public LetterHistoryController(ILetterHistoryService letterHistoryService)
    {
        _letterHistoryService = letterHistoryService;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var history = await _letterHistoryService.GetAllAsync(cancellationToken);

        return Ok(history);
    }

    [HttpGet("{processId}/file-url")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetFileUrlAsync(string processId, [FromQuery] string fileName, CancellationToken cancellationToken)
    {
        var url = await _letterHistoryService.GenerateFileUrlAsync(processId, fileName, cancellationToken);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }
}
