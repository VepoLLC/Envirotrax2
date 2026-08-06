using AutoMapper;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi
{
    public class CsiInspectorService : Service<Professional, ProfessionalDto>, ICsiInspectorService
    {
        private readonly ICsiInspectorRepository _inspectorRepository;

        public CsiInspectorService(IMapper mapper, ICsiInspectorRepository repository)
            : base(mapper, repository)
        {
            _inspectorRepository = repository;
        }

        public async Task<IPagedData<ProfessionalDto>> SearchAsync(string? inspectorLicenseNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken)
        {
            var items = await _inspectorRepository.SearchAsync(inspectorLicenseNumber, insurancePolicyNumber, pageInfo, cancellationToken);
            return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
        }
    }
}
