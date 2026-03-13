
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Users
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IMapper _mapper;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IAuthService _authService;

        public RolePermissionService(
            IMapper mapper,
            IRolePermissionRepository rolePermissionRepository,
            IAuthService authService)
        {
            _mapper = mapper;
            _rolePermissionRepository = rolePermissionRepository;
            _authService = authService;
        }

        public async Task<IEnumerable<ReferencedPermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _rolePermissionRepository.GetAllPermissionsAsync();
            return _mapper.Map<IEnumerable<Permission>, IEnumerable<ReferencedPermissionDto>>(permissions);
        }

        public async Task<IEnumerable<RolePermissionDto>> GetAllAsync(int roleId)
        {
            var rolePermissions = await _rolePermissionRepository.GetAllAsync(roleId);
            return _mapper.Map<IEnumerable<RolePermission>, IEnumerable<RolePermissionDto>>(rolePermissions);
        }

        public async Task<RolePermissionDto> AddOrUpdateAsync(RolePermissionDto rolePermission)
        {
            var model = _mapper.Map<RolePermissionDto, RolePermission>(rolePermission);

            var result = await _rolePermissionRepository.AddOrUpdateAsync(model);

            return _mapper.Map<RolePermission, RolePermissionDto>(result);
        }

        public async Task<IEnumerable<RolePermissionDto>> BulkUpdateAsync(IEnumerable<RolePermissionDto> rolePermissions)
        {
            var modelList = _mapper.Map<IEnumerable<RolePermissionDto>, IEnumerable<RolePermission>>(rolePermissions);

            var updatedList = await _rolePermissionRepository.BulkUpdateAsync(modelList);

            return _mapper.Map<IEnumerable<RolePermission>, IEnumerable<RolePermissionDto>>(updatedList);
        }
    }
}