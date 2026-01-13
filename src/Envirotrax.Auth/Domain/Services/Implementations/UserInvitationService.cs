
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Auth.Domain.Services.Definitions;
using Envirotrax.Auth.Templates.Emails;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Auth.Domain.Services.Implementations;

public class UserInvitationService : IUserInvitationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IUserInvitationReppsitory _invitationRepository;
    private readonly IEmailService _emailService;

    public UserInvitationService(
        UserManager<AppUser> userManager,
        IPasswordHasher<AppUser> passwordHasher,
        IUserInvitationReppsitory invitationReppsitory,
        IEmailService emailService)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _invitationRepository = invitationReppsitory;
        _emailService = emailService;
    }

    private async Task<AppUser> GetOrCreateUserIdAsync(AppUser? user, UserInvitationDto invitation)
    {
        if (user != null)
        {
            return user;
        }

        var identityResult = await _userManager.CreateAsync(new()
        {
            UserName = invitation.EmailAddress,
            Email = invitation.EmailAddress,
            EmailConfirmed = false
        });

        if (identityResult.Succeeded)
        {
            return (await _userManager.FindByEmailAsync(invitation.EmailAddress))!;
        }

        throw new ValidationException(string.Join(Environment.NewLine, identityResult.Errors));

    }

    public async Task<UserInvitationDto> AddAsync(UserInvitationDto invitation, int creatorId, IUrlHelper urlHelper)
    {
        var user = await _userManager.FindByEmailAsync(invitation.EmailAddress);

        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            user = await GetOrCreateUserIdAsync(user, invitation);

            var token = Guid.NewGuid().ToString();

            var addedInvitation = await _invitationRepository.AddAsync(new()
            {
                UserId = user.Id,
                CreatedById = creatorId,
                CreatedTime = DateTime.UtcNow,
                TokenHash = _passwordHasher.HashPassword(user, token)
            });
            invitation.Id = addedInvitation.Id;

            var callbackUrl = urlHelper.Page(
                 "/Account/AcceptInvitation",
                 pageHandler: null,
                 values: new { area = "Identity", tokenId = addedInvitation.Id, token },
                 protocol: "https");

            await _emailService.SendAsync<RegistrationInvitationVm>(new()
            {
                TemplateId = "RegistrationInvitation",
                Subject = "Registration Invitation",
                Recipients = [user.Email!],
                TemplateData = new()
                {
                    CallbackUrl = callbackUrl!
                }
            });

            transaction.Complete();
            return invitation;
        }
    }

    public async Task<bool> IsValidAsync(int invitationId, string token)
    {
        var invitation = await _invitationRepository.GetAsync(invitationId);

        if (invitation != null)
        {
            if (DateTime.UtcNow <= invitation.CreatedTime.AddDays(7))
            {
                if (_passwordHasher.VerifyHashedPassword(invitation.User!, invitation.TokenHash, token) == PasswordVerificationResult.Success)
                {
                    return true;
                }
            }
        }

        return false;
    }
}