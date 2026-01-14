
using System.ComponentModel.DataAnnotations;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Services.Definitions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Envirotrax.Auth.Areas.Identity.Pages.Account;

public class AcceptInvitationModel : PageModel
{
    private readonly IUserInvitationService _invitationService;
    private readonly UserManager<AppUser> _userManager;

    public bool IsSucceeded { get; set; }

    [BindProperty]
    public int TokenId { get; set; }

    [BindProperty]
    [Required]
    public string Token { get; set; } = null!;

    [BindProperty]
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [BindProperty]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = null!;

    public AcceptInvitationModel(IUserInvitationService invitationService, UserManager<AppUser> userManager)
    {
        _invitationService = invitationService;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(int tokenId, string token)
    {
        TokenId = tokenId;
        Token = Token;

        var result = await _invitationService.ValidateAsync(tokenId, token);

        if (!result.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Invitiation either expired or is invalid. Please contact your system administrator.");
            return Page();
        }

        if (result.User != null)
        {
            if (!string.IsNullOrWhiteSpace(result.User.PasswordHash))
            {
                IsSucceeded = true;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var result = await _invitationService.ValidateAsync(TokenId, Token);

            if (!result.IsValid)
            {
                ModelState.AddModelError("", "Invitiation either expired or is invalid. Please contact your system administrator.");
                return Page();
            }

            if (result.User == null)
            {
                throw new InvalidOperationException("Invitation validation result must reture user who it belongs to.");
            }

            var passwordResult = await _userManager.AddPasswordAsync(result.User, Password);

            if (passwordResult.Succeeded)
            {
                IsSucceeded = true;
                return Page();
            }

            foreach (var error in passwordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }
}