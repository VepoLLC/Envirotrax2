
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using Envirotrax.Auth.Data;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Auth.Domain.Services.Definitions;
using Envirotrax.Auth.Templates.Emails;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Domain.Services.Implementations;

public class UserInvitationService : IUserInvitationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IUserInvitationReppsitory _invitationRepository;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IEmailService _emailService;
    private readonly Definitions.IAuthService _authService;
    private readonly ApplicationDbContext _dbContext;

    public UserInvitationService(
        UserManager<AppUser> userManager,
        IPasswordHasher<AppUser> passwordHasher,
        IUserInvitationReppsitory invitationReppsitory,
        LinkGenerator linkGenerator,
        IHttpContextAccessor contextAccessor,
        IEmailService emailService,
        Definitions.IAuthService authService,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _invitationRepository = invitationReppsitory;
        _linkGenerator = linkGenerator;
        _contextAccessor = contextAccessor;
        _emailService = emailService;
        _authService = authService;
        _dbContext = dbContext;
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

    public async Task<UserInvitationDto> AddAsync(UserInvitationDto invitation)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            _dbContext.ChangeTracker.Clear();

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var existingUser = await _userManager.FindByEmailAsync(invitation.EmailAddress);
            var user = await GetOrCreateUserIdAsync(existingUser, invitation);

            var token = Guid.NewGuid().ToString();

            var addedInvitation = await _invitationRepository.AddAsync(new()
            {
                UserId = user.Id,
                CreatedById = _authService.UserId,
                CreatedTime = DateTime.UtcNow,
                TokenHash = _passwordHasher.HashPassword(user, token)
            });

            invitation.Id = addedInvitation.Id;
            invitation.UserId = addedInvitation.UserId;

            var callbackUrl = _linkGenerator.GetUriByPage(
                httpContext: _contextAccessor.HttpContext!,
                page: "/Account/AcceptInvitation",
                handler: null,
                values: new { area = "Identity", tokenId = addedInvitation.Id, token },
                scheme: "https"
            );

            await _emailService.SendAsync<RegistrationInvitationVm>(new()
            {
                TemplateId = "RegistrationInvitation",
                Subject = "Registration Invitation",
                Recipients = [user.Email!],
                TemplateData = new()
                {
                    CallbackUrl = callbackUrl!,
                    InvitedByCompany = invitation.InvitedByCompany
                }
            });

            transaction.Complete();

            return invitation;
        });
    }

    public async Task<InvitationValidationResultDto> ValidateAsync(int invitationId, string token)
    {
        var result = new InvitationValidationResultDto();
        var invitation = await _invitationRepository.GetAsync(invitationId);

        if (invitation != null)
        {
            if (DateTime.UtcNow <= invitation.CreatedTime.AddDays(7))
            {
                if (_passwordHasher.VerifyHashedPassword(invitation.User!, invitation.TokenHash, token) == PasswordVerificationResult.Success)
                {
                    result.IsValid = true;
                    result.User = invitation.User;
                }
            }
        }

        return result;
    }

    public async Task DeleteAllAsync(int userId)
    {
        await _invitationRepository.DeleteAllAsync(userId);
    }
}