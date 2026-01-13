
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data.Repositories.Implementations;

public class UserInvitationReppsitory : IUserInvitationReppsitory
{
    private readonly ApplicationDbContext _dbContext;

    public UserInvitationReppsitory(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserInvitation?> GetAsync(int id)
    {
        return await _dbContext
            .UserInvitations
            .Include(invitation => invitation.User)
            .SingleOrDefaultAsync(invitation => invitation.Id == id);
    }

    public async Task<UserInvitation> AddAsync(UserInvitation invitation)
    {
        _dbContext.UserInvitations.Add(invitation);
        await _dbContext.SaveChangesAsync();

        return invitation;
    }
}