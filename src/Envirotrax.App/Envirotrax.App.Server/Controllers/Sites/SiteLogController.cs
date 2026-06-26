using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Sites;

[Route("api/sites/{siteId}/logs")]
[PermissionResource(PermissionType.Sites)]
public class SiteLogController : WaterSupplierProtectedController
{
    private readonly ISiteLogService _service;

    public SiteLogController(ISiteLogService service)
    {
        _service = service;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetBySiteAsync(int siteId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _service.GetBySiteAsync(siteId, pageInfo, query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> AddAsync(int siteId, [FromForm] SiteLogDto dto, [FromForm] IFormFile? file, CancellationToken cancellationToken)
    {
        Stream? fileStream = null;
        string? fileName = null;
        if (file != null)
        {
            fileStream = file.OpenReadStream();
            fileName = file.FileName;
        }
        await using (fileStream)
        {
            var result = await _service.AddAsync(siteId, dto, fileStream, fileName);
            return Ok(result);
        }
    }

    [HttpPut("{id}")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateAsync(int siteId, int id, [FromForm] SiteLogDto dto, [FromForm] IFormFile? file, CancellationToken cancellationToken)
    {
        dto.Id = id;
        Stream? fileStream = null;
        string? fileName = null;
        if (file != null)
        {
            fileStream = file.OpenReadStream();
            fileName = file.FileName;
        }
        await using (fileStream)
        {
            var result = await _service.UpdateAsync(dto, fileStream, fileName, cancellationToken);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }

    [HttpDelete("{id}")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> DeleteAsync(int siteId, int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
