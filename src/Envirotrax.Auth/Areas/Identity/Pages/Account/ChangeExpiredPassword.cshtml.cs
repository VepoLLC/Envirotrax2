#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Envirotrax.Auth.Areas.Identity.Pages.Account
{
    [Authorize]
    [AllowExpiredPassword]
    public class ChangeExpiredPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<ChangeExpiredPasswordModel> _logger;

        public ChangeExpiredPasswordModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<ChangeExpiredPasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.PasswordExpirationDate.HasValue || user.PasswordExpirationDate.Value > DateTime.UtcNow)
            {
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (_userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, Input.NewPassword) == PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Your new password cannot be the same as your current password.");
                return Page();
            }

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            var addResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            // One-time forced change for legacy-migrated accounts with potentially weak/compromised
            // hashes. Cleared (not rolled forward) because mandatory periodic rotation is no longer
            // recommended practice (e.g. NIST 800-63B) — it tends to push users toward weaker,
            // predictable passwords rather than improving security.
            user.PasswordExpirationDate = null;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their expired password successfully.");

            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
    }
}
