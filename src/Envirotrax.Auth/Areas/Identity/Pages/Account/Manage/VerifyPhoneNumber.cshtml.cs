// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Envirotrax.Auth.Areas.Identity.Pages.Account.Manage
{
    public class VerifyPhoneNumberModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ISmsService _smsService;

        public VerifyPhoneNumberModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ISmsService smsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _smsService = smsService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string PhoneNumber { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Verification code")]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return RedirectToPage("./Index");
            }

            PhoneNumber = phoneNumber;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            PhoneNumber = phoneNumber;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var code = Input.Code.Replace(" ", string.Empty);

            var result = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, code);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid verification code.");
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your phone number has been verified.";
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostResendAsync(string phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            await _smsService.SendAsync(new SmsDto
            {
                To = phoneNumber,
                Body = $"Your Envirotrax phone verification code is {code}"
            });

            return RedirectToPage(new { phoneNumber });
        }
    }
}
