
using Envirotrax.App.Server.Data.Models.Users;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Users;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetAllAsync(int userId);

    Task<UserRole> AddAsync(UserRole userRole);

    Task<UserRole?> DeleteAsync(int userId, int roleId);
}