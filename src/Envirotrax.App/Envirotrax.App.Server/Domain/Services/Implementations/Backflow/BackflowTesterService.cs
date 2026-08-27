using AutoMapper;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow
{
    public class BackflowTesterService : Service<Professional, ProfessionalDto>, IBackflowTesterService
    {
        private readonly IBackflowTesterRepository _testerRepository;

        public BackflowTesterService(IMapper mapper, IBackflowTesterRepository repository)
            : base(mapper, repository)
        {
            _testerRepository = repository;
        }

        public async Task<IPagedData<ProfessionalDto>> SearchAsync(string? bpatLicenseNumber, string? fireLicenseNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken)
        {
            var items = await _testerRepository.SearchAsync(bpatLicenseNumber, fireLicenseNumber, insurancePolicyNumber, pageInfo, cancellationToken);
            return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
        }
    }
}
