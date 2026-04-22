
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalUserService : IService<ProfessionalUser, ProfessionalUserDto>
{
    Task<ProfessionalUserDto?> GetMyDataAsync(CancellationToken cancellationToken);
    Task<ProfessionalUserDto?> UpdateMyDataAsync(ProfessionalUserDto user);

    Task<ProfessionalUserDto?> ResendInvitationAsync(int id);
    Task<IPagedData<ProfessionalUserDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}