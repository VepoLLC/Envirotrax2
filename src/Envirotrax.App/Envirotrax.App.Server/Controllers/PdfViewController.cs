using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Envirotrax.App.Server.Controllers;

[Route("api/pdf-views")]
public class PdfViewController : EnvirotraxBaseController
{
    private const int MaxPdfSizeBytes = 50 * 1024 * 1024;

    private readonly IPdfViewService _pdfViewService;

    public PdfViewController(IPdfViewService pdfViewService)
    {
        _pdfViewService = pdfViewService;
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleDefinitions.WaterSupplier},{RoleDefinitions.Professional}")]
    [RequestSizeLimit(MaxPdfSizeBytes)]
    public async Task<IActionResult> CreateAsync([FromQuery] string fileName, CancellationToken cancellationToken)
    {
        using var buffer = new MemoryStream();
        await Request.Body.CopyToAsync(buffer, cancellationToken);

        var view = _pdfViewService.Store(buffer.ToArray(), fileName);

        return Ok(new PdfViewLinkDto
        {
            Url = $"/api/pdf-views/{view.Token}/{Uri.EscapeDataString(view.FileName)}"
        });
    }

    [HttpGet("{token}/{fileName}")]
    [AllowAnonymous]
    public IActionResult Get(string token, string fileName)
    {
        var view = _pdfViewService.Find(token);

        if (view == null)
        {
            return NotFound();
        }

        Response.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = view.FileName }.ToString();
        Response.Headers.XContentTypeOptions = "nosniff";

        return File(view.Content, "application/pdf");
    }
}
