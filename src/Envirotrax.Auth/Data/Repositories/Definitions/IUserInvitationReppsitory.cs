
using Envirotrax.Auth.Data.Models;

namespace Envirotrax.Auth.Data.Repositories.Defintions;

public interface IUserInvitationReppsitory
{
    Task<UserInvitation?> GetAsync(int id);

    Task<UserInvitation> AddAsync(UserInvitation invitation);

    Task DeleteAllAsync(int userId);
}