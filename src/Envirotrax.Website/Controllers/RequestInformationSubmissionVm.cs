using Envirotrax.Website.Templates.Emails;

namespace Envirotrax.Website.Controllers;

public class RequestInformationSubmissionVm : RequestInformationVm
{
    public string RecaptchaToken { get; set; } = null!;
}
