
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Users;

public class UserReoleRepository : IUserRoleRepository
{
    private readonly TenantDbContext _dbContext;

    public UserReoleRepository(IDbContextSelector dbContextSelector)
    {
        _dbContext = dbContextSelector.Current;
    }

    public async Task<IEnumerable<UserRole>> GetAllAsync(int userId)
    {
        return await _dbContext
            .UserRoles
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .Include(r => r.Role)
            .ToListAsync();
    }

    public async Task<UserRole> AddAsync(UserRole userRole)
    {
        _dbContext.UserRoles.Add(userRole);
        await _dbContext.SaveChangesAsync();

        return userRole;
    }

    public async Task<UserRole?> DeleteAsync(int userId, int roleId)
    {
        var userRole = await _dbContext
            .UserRoles
            .SingleOrDefaultAsync(r => r.UserId == userId && r.RoleId == roleId);

        if (userRole != null)
        {
            _dbContext.UserRoles.Remove(userRole);
            await _dbContext.SaveChangesAsync();
        }

        return userRole;
    }
}