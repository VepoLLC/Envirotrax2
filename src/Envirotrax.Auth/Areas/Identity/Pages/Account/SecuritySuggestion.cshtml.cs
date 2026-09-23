#nullable disable

using System;
using System.Threading.Tasks;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Envirotrax.Auth.Areas.Identity.Pages.Account
{
    public class SecuritySuggestionModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public SecuritySuggestionModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public string Title { get; set; }

        public string Message { get; set; }

        public string PrimaryActionUrl { get; set; }

        public string PrimaryActionText { get; set; }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(SecuritySuggestionType type, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            var user = await _userManager.GetUserAsync(User);

            switch (type)
            {
                case SecuritySuggestionType.PhoneNumber:
                    if (await _userManager.IsPhoneNumberConfirmedAsync(user))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    Title = "Add a phone number";
                    Message = "Adding a phone number lets us send your two-factor sign-in codes by text message instead of email, so you don't have to wait on an email to arrive every time you log in.";
                    PrimaryActionUrl = Url.Page("./Manage/Index");
                    PrimaryActionText = "Add phone number";
                    break;

                default:
                    return LocalRedirect(returnUrl);
            }

            return Page();
        }

        public IActionResult OnGetSkip(string returnUrl)
        {
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
    }
}
