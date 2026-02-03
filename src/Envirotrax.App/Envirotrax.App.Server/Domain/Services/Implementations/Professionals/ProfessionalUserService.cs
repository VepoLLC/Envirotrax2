
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalUserService : Service<ProfessionalUser, ProfessionalUserDto>, IProfessionalUserService
{
    private readonly IAuthService _authService;

    public ProfessionalUserService(
        IMapper mapper,
        IProfessionalUserRepository repository,
        IAuthService authService)
        : base(mapper, repository)
    {
        _authService = authService;
    }

    public Task<ProfessionalUserDto?> GetMyDataAsync(CancellationToken cancellationToken)
    {
        return GetAsync(_authService.UserId, cancellationToken);
    }

    public Task<ProfessionalUserDto> UpdateMyDataAsync(ProfessionalUserDto user)
    {
        user.Id = _authService.UserId;
        return UpdateAsync(user);
    }
}