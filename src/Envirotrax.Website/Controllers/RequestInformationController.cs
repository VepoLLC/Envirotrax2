using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.Website.Templates.Emails;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Website.Controllers;

[ApiController]
[Route("api/request-information")]
public class RequestInformationController : ControllerBase
{
    private readonly IEmailService _emailService;

    public RequestInformationController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] RequestInformationVm submission)
    {
        if (string.IsNullOrWhiteSpace(submission.CompanyName) ||
            string.IsNullOrWhiteSpace(submission.ContactName) ||
            string.IsNullOrWhiteSpace(submission.InformationType))
        {
            return BadRequest();
        }

        await _emailService.SendAsync(new EmailDto<RequestInformationVm>
        {
            FromAddress = FromAddressType.Team,
            Recipients = ["team@envirotrax.com"],
            Subject = $"Envirotrax Request for Information - {submission.CompanyName}",
            TemplateId = "RequestInformation",
            TemplateData = submission
        });

        return Ok();
    }
}
