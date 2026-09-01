using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Website.Controllers;

/// <summary>
/// The registered professional directories live in Envirotrax.App, not in the CMS. This controller
/// keeps the marketing site's own /registered-professionals/... URLs working by forwarding them to
/// the matching public page in the app, so existing links and bookmarks do not break.
/// </summary>
[Route("registered-professionals")]
public class RegisteredProfessionalsController : ControllerBase
{
    /// <summary>
    /// The directories the app publishes. Redirecting only to a known slug keeps this from becoming
    /// an open redirect.
    /// </summary>
    private static readonly string[] AccountTypes =
    [
        "backflow-testers",
        "csi-inspectors",
        "fog-inspectors",
        "fog-transporters"
    ];

    private readonly IConfiguration _configuration;

    public RegisteredProfessionalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToApp(string.Empty);
    }

    [HttpGet("{accountType}")]
    public IActionResult Search(string accountType)
    {
        if (!AccountTypes.Contains(accountType, StringComparer.OrdinalIgnoreCase))
        {
            return NotFound();
        }

        return RedirectToApp(accountType);
    }

    private IActionResult RedirectToApp(string accountType)
    {
        var appUrl = _configuration["Envirotrax:AppUrl"];

        if (string.IsNullOrWhiteSpace(appUrl))
        {
            return NotFound();
        }

        var url = $"{appUrl.TrimEnd('/')}/registered-professionals";

        if (!string.IsNullOrEmpty(accountType))
        {
            url += $"/{accountType}";
        }

        return Redirect(url);
    }
}
