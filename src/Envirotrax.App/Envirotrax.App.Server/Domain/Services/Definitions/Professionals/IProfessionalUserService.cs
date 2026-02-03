
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalUserService : IService<ProfessionalUser, ProfessionalUserDto>
{
    Task<ProfessionalUserDto?> GetMyDataAsync(CancellationToken cancellationToken);
    Task<ProfessionalUserDto> UpdateMyDataAsync(ProfessionalUserDto user);
}