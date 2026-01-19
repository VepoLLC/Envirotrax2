
using Envirotrax.Auth.Domain.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Auth.Domain.Services.Definitions;

public interface IUserInvitationService
{
    Task<UserInvitationDto> AddAsync(UserInvitationDto invitation);

    Task<InvitationValidationResultDto> ValidateAsync(int invitationId, string token);

    Task DeleteAllAsync(int userId);
}