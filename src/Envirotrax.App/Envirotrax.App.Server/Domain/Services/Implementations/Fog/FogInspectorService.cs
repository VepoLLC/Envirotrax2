using AutoMapper;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog
{
    public class FogInspectorService : Service<Professional, ProfessionalDto>, IFogInspectorService
    {
        private readonly IFogInspectorRepository _inspectorRepository;

        public FogInspectorService(IMapper mapper, IFogInspectorRepository repository)
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
