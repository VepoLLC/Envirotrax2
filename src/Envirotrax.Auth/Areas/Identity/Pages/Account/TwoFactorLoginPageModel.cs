#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Envirotrax.Auth.Areas.Identity.Pages.Account
{
    public abstract class TwoFactorLoginPageModel : PageModel
    {
        protected IActionResult RedirectToExpiredLogin(string returnUrl)
        {
            TempData["ErrorMessage"] = "Your login session has expired. Please log in again.";

            return RedirectToPage("./Login", new { returnUrl });
        }
    }
}
