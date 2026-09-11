using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow
{
    public interface IBackflowTesterService : IService<Professional, ProfessionalDto>
    {
        Task<IPagedData<ProfessionalDto>> SearchAsync(
            string? bpatLicenseNumber,
            string? fireLicenseNumber,
            string? insurancePolicyNumber,
            string? userEmail,
            string? contactName,
            string? cellNumber,
            PageInfo pageInfo,
            Query query,
            CancellationToken cancellationToken);
    }
}
