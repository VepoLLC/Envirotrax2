
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Backflow;

[Route("api/professionals/backflow/gauges")]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.BackflowTester}")]
public class BackflowGaugesController : ProfessionalCrudController<BackflowGaugeDto>
{
    private readonly IBackflowGaugeService _gaugeService;
    private readonly IFileStorageService _fileStorageService;

    public BackflowGaugesController(IBackflowGaugeService service, IFileStorageService fileStorageService)
        : base(service)
    {
        _gaugeService = service;
        _fileStorageService = fileStorageService;
    }

    [NonAction]
    public override Task<IActionResult> AddAsync(BackflowGaugeDto dto)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromForm] CreateBackflowGaugeDto gauge)
    {
        await using var stream = gauge.File.OpenReadStream();
        var result = await _gaugeService.AddWithFileAsync(stream, gauge.File.FileName, gauge);
        return Ok(result);
    }

    [HttpGet("{id}/file")]
    public async Task<IActionResult> GetFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        var gauge = await _gaugeService.GetAsync(id, cancellationToken);
        if (gauge?.FilePath == null)
            return NotFound();

        var url = await _fileStorageService.GenerateSasUrlAsync(gauge.FilePath);
        return Ok(url);
    }
}
