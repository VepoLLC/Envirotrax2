// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Templates.Emails;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Envirotrax.Auth.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public LoginWith2faModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IEmailService emailService,
            ISmsService smsService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _smsService = smsService;
        }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public bool HasAuthenticator { get; set; }

        public bool HasSms { get; set; }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            var user = await GetTwoFactorUserAsync();

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null && await _userManager.GetTwoFactorEnabledAsync(user);
            HasSms = await _userManager.IsPhoneNumberConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostSendEmailAsync(bool rememberMe, string returnUrl = null)
        {
            var user = await GetTwoFactorUserAsync();

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            await _emailService.SendAsync<MfaCodeVm>(new()
            {
                TemplateId = "MfaCode",
                Subject = "Your verification code",
                Recipients = [user.Email],
                TemplateData = new()
                {
                    Code = code
                }
            });

            return RedirectToPage("./LoginWith2faEmail", new { returnUrl, rememberMe });
        }

        public async Task<IActionResult> OnPostSendSmsAsync(bool rememberMe, string returnUrl = null)
        {
            var user = await GetTwoFactorUserAsync();

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");

            await _smsService.SendAsync(new SmsDto
            {
                To = user.PhoneNumber,
                Body = $"Your Envirotrax verification code is {code}"
            });

            return RedirectToPage("./LoginWith2faSms", new { returnUrl, rememberMe });
        }

        private async Task<AppUser> GetTwoFactorUserAsync()
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            return user;
        }
    }
}
