using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog
{
    public interface IFogTransporterService : IService<Professional, ProfessionalDto>
    {
        Task<IPagedData<ProfessionalDto>> SearchAsync(string? registrationNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken);
        Task<IPagedData<ProfessionalDto>> GetAllWithDetailsAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    }
}
