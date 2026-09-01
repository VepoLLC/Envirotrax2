using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectorUserService
{
    Task<IPagedData<ProfessionalUserDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalUserDto?> AddAsync(int professionalId, ProfessionalUserDto user, CancellationToken cancellationToken);

    Task<ProfessionalUserDto?> UpdateAsync(int professionalId, int userId, ProfessionalUserDto user, CancellationToken cancellationToken);

    Task DeleteAsync(int professionalId, int userId, CancellationToken cancellationToken);
}
