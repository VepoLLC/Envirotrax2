using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/backflow/reports")]
[PermissionResource(PermissionType.BackflowReports)]
public class BackflowReportsController : WaterSupplierProtectedController
{
    private readonly IBackflowTestReportService _testReportService;
    private readonly IBackflowComplianceReportService _complianceReportService;
    private readonly IBackflowNewRemovedReportService _newRemovedReportService;

    public BackflowReportsController(
        IBackflowTestReportService testReportService,
        IBackflowComplianceReportService complianceReportService,
        IBackflowNewRemovedReportService newRemovedReportService)
    {
        _testReportService = testReportService;
        _complianceReportService = complianceReportService;
        _newRemovedReportService = newRemovedReportService;
    }

    [HttpGet("tests")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetTestReportAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var report = await _testReportService.GetTestReportAsync(fromDate, toDate, cancellationToken);
        return Ok(report);
    }

    [HttpGet("tests/pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetTestReportPdfAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var pdf = await _testReportService.GeneratePdfAsync(fromDate, toDate, cancellationToken);
        return File(pdf, MimeTypes.Pdf);
    }

    [HttpGet("tests/excel")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetTestReportExcelAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var excel = await _testReportService.GenerateExcelAsync(fromDate, toDate, cancellationToken);
        return File(excel, MimeTypes.Excel);
    }

    [HttpGet("tests/word")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetTestReportWordAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var word = await _testReportService.GenerateWordAsync(fromDate, toDate, cancellationToken);
        return File(word, MimeTypes.Word);
    }

    [HttpGet("tests/earliest-date")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetEarliestTestDateAsync(CancellationToken cancellationToken)
    {
        var earliestDate = await _testReportService.GetEarliestTestDateAsync(cancellationToken);
        return Ok(new { earliestDate });
    }

    [HttpGet("compliance/current")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetCurrentComplianceAsync(
        [FromQuery] bool ignoreLast30Days,
        CancellationToken cancellationToken)
    {
        var report = await _complianceReportService.GetComplianceReportAsync(ignoreLast30Days, cancellationToken);
        return Ok(report);
    }

    [HttpGet("compliance/history")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetComplianceHistoryAsync(CancellationToken cancellationToken)
    {
        var report = await _complianceReportService.GetComplianceHistoryAsync(cancellationToken);
        return Ok(report);
    }

    [HttpGet("assemblies/new-removed")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetNewRemovedAsync(CancellationToken cancellationToken)
    {
        var report = await _newRemovedReportService.GetNewRemovedAsync(cancellationToken);
        return Ok(report);
    }
}
