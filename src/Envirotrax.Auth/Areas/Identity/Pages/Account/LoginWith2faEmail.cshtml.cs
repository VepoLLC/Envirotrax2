// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Envirotrax.Auth.Areas.Identity.Pages.Account
{
    public class LoginWith2faEmailModel : TwoFactorLoginPageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginWith2faEmailModel> _logger;

        public LoginWith2faEmailModel(
            SignInManager<AppUser> signInManager,
            ILogger<LoginWith2faEmailModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Verification code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the code was actually sent (via LoginWith2fa's SendEmail handler) for this pending login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return RedirectToExpiredLogin(returnUrl);
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToExpiredLogin(returnUrl);
            }

            var code = Input.TwoFactorCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorSignInAsync("Email", code, rememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with an email 2fa code.", user.Id);

                if (!await _signInManager.UserManager.IsPhoneNumberConfirmedAsync(user))
                {
                    return RedirectToPage("./SecuritySuggestion", new { type = SecuritySuggestionType.PhoneNumber, returnUrl });
                }

                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid email verification code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid verification code.");
                return Page();
            }
        }
    }
}
