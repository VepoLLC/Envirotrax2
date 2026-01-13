
using Envirotrax.Auth.Domain.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Auth.Domain.Services.Definitions;

public interface IUserInvitationService
{
    Task<UserInvitationDto> AddAsync(UserInvitationDto invitation, int creatorId, IUrlHelper urlHelper);

    Task<bool> IsValidAsync(int invitationId, string token);
}