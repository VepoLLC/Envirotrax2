using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/backflow/tests")]
[PermissionResource(PermissionType.BackflowTests)]
public class BackflowTestController : WaterSupplierCrudController<BackflowTestDto>
{
    private readonly IBackflowTestService _testService;

    public BackflowTestController(IBackflowTestService service)
        : base(service)
    {
        _testService = service;
    }

    protected override Task<IPagedData<BackflowTestDto>> ProcessGetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var subAccountWaterSupplierId = ReadSubAccountWaterSupplierId();

        if (subAccountWaterSupplierId.HasValue)
        {
            return _testService.SearchForSubAccountAsync(pageInfo, query, subAccountWaterSupplierId.Value, cancellationToken);
        }

        return _testService.SearchAsync(pageInfo, query, ReadPaymentStatus(), cancellationToken);
    }

    private BackflowPaymentStatus? ReadPaymentStatus()
    {
        if (Enum.TryParse<BackflowPaymentStatus>(Request.Query["paymentStatus"], out var paymentStatus))
        {
            return paymentStatus;
        }

        return null;
    }

    private int? ReadSubAccountWaterSupplierId()
    {
        if (int.TryParse(Request.Query["subAccountWaterSupplierId"], out var waterSupplierId))
        {
            return waterSupplierId;
        }

        return null;
    }

    [HttpGet("pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllPdfAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        var tests = await _testService.SearchAsync(pageInfo, query, paymentStatus, cancellationToken);
        var pdf = await _testService.GeneratePdfAsync(tests.Data);
        return File(pdf, "application/pdf");
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdfAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _testService.GetAsync(id, cancellationToken);
        if (test == null)
        {
            return NotFound();
        }

        var pdf = await _testService.GeneratePdfAsync(test);
        return File(pdf, "application/pdf");
    }

    [HttpPut("{id}/renewal-required")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateRenewalRequiredAsync(int id, [FromBody] bool renewalRequired, CancellationToken cancellationToken)
    {
        var result = await _testService.UpdateRenewalRequiredAsync(id, renewalRequired, cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}/schedule-month")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateScheduleMonthAsync(int id, [FromBody] BackflowTestScheduleMonthRequest request, CancellationToken cancellationToken)
    {
        var result = await _testService.UpdateScheduleMonthAsync(id, request.Month, cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}/is-current")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateIsCurrentAsync(int id, [FromBody] bool isCurrent, CancellationToken cancellationToken)
    {
        var result = await _testService.UpdateIsCurrentAsync(id, isCurrent, cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}/out-of-service")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateOutOfServiceAsync(int id, [FromBody] bool outOfService, CancellationToken cancellationToken)
    {
        var result = await _testService.UpdateOutOfServiceAsync(id, outOfService, cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}/disapproval")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateDisapprovalAsync(int id, [FromBody] bool disapproved, CancellationToken cancellationToken)
    {
        var result = await _testService.UpdateDisapprovalAsync(id, disapproved, cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}/rejection")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateRejectionAsync(int id, [FromBody] BackflowTestRejectionRequest request, CancellationToken cancellationToken)
    {
        var result = await _testService.UpdateRejectionAsync(id, request, cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}/force-renewal")]
    [HasPermission(PermissionAction.CanModify)]
    [HasFeature(FeatureType.BackflowTestForceRenewal)]
    public async Task<IActionResult> UpdateForceRenewalAsync(int id, [FromBody] BackflowTestForceRenewalRequest request, CancellationToken cancellationToken)
    {

        // not implemented isAdmin check: V1 used IsAdmin = WaterSupplierUserAccounts.IsVepoAdministrator
        var result = await _testService.UpdateForceRenewalAsync(id, request, cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("{id}/images/{imageType}")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UploadImageAsync(int id, string imageType, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _testService.UpdateImageAsync(id, imageType, stream, file.FileName, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
