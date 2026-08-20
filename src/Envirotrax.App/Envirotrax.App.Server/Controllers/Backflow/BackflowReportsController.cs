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

    [HttpGet("compliance/current/pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetCurrentCompliancePdfAsync(
        [FromQuery] bool ignoreLast30Days,
        CancellationToken cancellationToken)
    {
        var pdf = await _complianceReportService.GeneratePdfAsync(ignoreLast30Days, cancellationToken);
        return File(pdf, MimeTypes.Pdf);
    }

    [HttpGet("compliance/current/excel")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetCurrentComplianceExcelAsync(
        [FromQuery] bool ignoreLast30Days,
        CancellationToken cancellationToken)
    {
        var excel = await _complianceReportService.GenerateExcelAsync(ignoreLast30Days, cancellationToken);
        return File(excel, MimeTypes.Excel);
    }

    [HttpGet("compliance/current/word")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetCurrentComplianceWordAsync(
        [FromQuery] bool ignoreLast30Days,
        CancellationToken cancellationToken)
    {
        var word = await _complianceReportService.GenerateWordAsync(ignoreLast30Days, cancellationToken);
        return File(word, MimeTypes.Word);
    }

    [HttpGet("compliance/history")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetComplianceHistoryAsync(CancellationToken cancellationToken)
    {
        var report = await _complianceReportService.GetComplianceHistoryAsync(cancellationToken);
        return Ok(report);
    }

    [HttpGet("compliance/history/pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetComplianceHistoryPdfAsync(CancellationToken cancellationToken)
    {
        var pdf = await _complianceReportService.GenerateHistoryPdfAsync(cancellationToken);
        return File(pdf, MimeTypes.Pdf);
    }

    [HttpGet("compliance/history/excel")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetComplianceHistoryExcelAsync(CancellationToken cancellationToken)
    {
        var excel = await _complianceReportService.GenerateHistoryExcelAsync(cancellationToken);
        return File(excel, MimeTypes.Excel);
    }

    [HttpGet("compliance/history/word")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetComplianceHistoryWordAsync(CancellationToken cancellationToken)
    {
        var word = await _complianceReportService.GenerateHistoryWordAsync(cancellationToken);
        return File(word, MimeTypes.Word);
    }

    [HttpGet("assemblies/new-removed")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetNewRemovedAsync(CancellationToken cancellationToken)
    {
        var report = await _newRemovedReportService.GetNewRemovedAsync(cancellationToken);
        return Ok(report);
    }

    [HttpGet("assemblies/new-removed/pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetNewRemovedPdfAsync(CancellationToken cancellationToken)
    {
        var pdf = await _newRemovedReportService.GeneratePdfAsync(cancellationToken);
        return File(pdf, MimeTypes.Pdf);
    }

    [HttpGet("assemblies/new-removed/excel")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetNewRemovedExcelAsync(CancellationToken cancellationToken)
    {
        var excel = await _newRemovedReportService.GenerateExcelAsync(cancellationToken);
        return File(excel, MimeTypes.Excel);
    }

    [HttpGet("assemblies/new-removed/word")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetNewRemovedWordAsync(CancellationToken cancellationToken)
    {
        var word = await _newRemovedReportService.GenerateWordAsync(cancellationToken);
        return File(word, MimeTypes.Word);
    }
}
