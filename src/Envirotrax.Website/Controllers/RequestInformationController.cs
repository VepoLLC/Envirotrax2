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
    private readonly IRecaptchaVerificationService _recaptchaVerificationService;

    public RequestInformationController(IEmailService emailService, IRecaptchaVerificationService recaptchaVerificationService)
    {
        _emailService = emailService;
        _recaptchaVerificationService = recaptchaVerificationService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] RequestInformationSubmissionVm submission)
    {
        if (string.IsNullOrWhiteSpace(submission.CompanyName) ||
            string.IsNullOrWhiteSpace(submission.ContactName) ||
            string.IsNullOrWhiteSpace(submission.InformationType))
        {
            return BadRequest();
        }

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var recaptchaVerified = await _recaptchaVerificationService.VerifyAsync(submission.RecaptchaToken, remoteIp);

        if (!recaptchaVerified)
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
