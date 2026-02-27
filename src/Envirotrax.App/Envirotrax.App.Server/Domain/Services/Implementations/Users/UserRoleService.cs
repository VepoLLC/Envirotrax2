
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class UserRoleService : IUserRoleService
{
    private readonly IMapper _mapper;
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleService(IMapper mapper, IUserRoleRepository userRoleRepository)
    {
        _mapper = mapper;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<IEnumerable<UserRoleDto>> GetAllAsync(int userId)
    {
        var userRoles = await _userRoleRepository.GetAllAsync(userId);
        return _mapper.Map<IEnumerable<UserRole>, IEnumerable<UserRoleDto>>(userRoles);
    }

    public async Task<UserRoleDto> AddAsync(UserRoleDto userReol)
    {
        var model = _mapper.Map<UserRoleDto, UserRole>(userReol);

        var added = await _userRoleRepository.AddAsync(model);

        return _mapper.Map<UserRole, UserRoleDto>(added);
    }

    public async Task<UserRoleDto?> DeleteAsync(int userId, int roleId)
    {
        var deleted = await _userRoleRepository.DeleteAsync(userId, roleId);

        if (deleted != null)
        {
            return _mapper.Map<UserRole, UserRoleDto>(deleted);
        }

        return null;
    }
}