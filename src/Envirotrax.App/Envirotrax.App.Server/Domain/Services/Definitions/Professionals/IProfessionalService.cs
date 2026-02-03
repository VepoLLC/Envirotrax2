
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalService : IService<Professional, ProfessionalDto>
{
    Task<IPagedData<ProfessionalDto>> GetAllMyAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ProfessionalDto?> GetLoggedInProfessionalAsync(CancellationToken cancellationToken);

    Task<ProfessionalDto> AddMyAsync(CreateProfessionalDto createProfessional);
}