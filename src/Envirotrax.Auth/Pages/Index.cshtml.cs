using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Envirotrax.Auth.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;

    public IndexModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult OnGet()
    {
        var url = _configuration["Sites:Envirotrax.App:Url"];

        if (!string.IsNullOrEmpty(url))
        {
            return Redirect(url);
        }

        return Page();
    }
}
