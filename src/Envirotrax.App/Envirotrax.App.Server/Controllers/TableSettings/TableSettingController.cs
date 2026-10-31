using Envirotrax.App.Server.Data.Models.TableSettings;
using Envirotrax.App.Server.Domain.DataTransferObjects.TableSettings;
using Envirotrax.App.Server.Domain.Services.Definitions.TableSettings;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.TableSettings;

// No permission resource on purpose: every water supplier user manages only their own table settings,
// and the service limits each call to them.
[Route("api/table-settings")]
public class TableSettingController : WaterSupplierProtectedController
{
    private readonly ITableSettingService _service;

    public TableSettingController(ITableSettingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMyAsync([FromQuery] TableSettingType tableType, CancellationToken cancellationToken)
    {
        var settings = await _service.GetAllMyAsync(tableType, cancellationToken);
        return Ok(settings);
    }

    [HttpGet("availability")]
    public async Task<IActionResult> CanAddMyAsync(CancellationToken cancellationToken)
    {
        var canAdd = await _service.CanAddMyAsync(cancellationToken);
        return Ok(canAdd);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(TableSettingDto dto, CancellationToken cancellationToken)
    {
        var added = await _service.AddMyAsync(dto, cancellationToken);
        return Ok(added);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, TableSettingDto dto, CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateMyAsync(id, dto, cancellationToken);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteMyAsync(id, cancellationToken);

        if (deleted == null)
        {
            return NotFound();
        }

        return Ok(deleted);
    }
}
