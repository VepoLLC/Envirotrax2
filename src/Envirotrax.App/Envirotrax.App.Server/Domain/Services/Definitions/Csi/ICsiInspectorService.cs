using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi
{
    public interface ICsiInspectorService : IService<Professional, ProfessionalDto>
    {
        Task<IPagedData<ProfessionalDto>> SearchAsync(string? inspectorLicenseNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken);
    }
}