
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Users;

public interface IUserRoleService
{
    Task<IEnumerable<UserRoleDto>> GetAllAsync(int userId);

    Task<UserRoleDto> AddAsync(UserRoleDto userReol);

    Task<UserRoleDto?> DeleteAsync(int userId, int roleId);
}