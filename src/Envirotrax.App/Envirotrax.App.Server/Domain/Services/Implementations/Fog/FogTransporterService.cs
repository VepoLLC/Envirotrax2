using AutoMapper;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog
{
    public class FogTransporterService : Service<Professional, ProfessionalDto>, IFogTransporterService
    {
        private readonly IFogTransporterRepository _transporterRepository;
        private readonly IProfessionalUserLicenseService _licenseService;
        private readonly IProfessionalInsuranceService _insuranceService;

        public FogTransporterService(
            IMapper mapper,
            IFogTransporterRepository repository,
            IProfessionalUserLicenseService licenseService,
            IProfessionalInsuranceService insuranceService)
            : base(mapper, repository)
        {
            _transporterRepository = repository;
            _licenseService = licenseService;
            _insuranceService = insuranceService;
        }

        public async Task<IPagedData<ProfessionalDto>> SearchAsync(string? registrationNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken)
        {
            var items = await _transporterRepository.SearchAsync(registrationNumber, insurancePolicyNumber, pageInfo, cancellationToken);
            var dtos = items.Select(i => MapToDto(i)!).ToList();

            await EnrichWithLicensesAndInsurancesAsync(dtos, cancellationToken);

            return dtos.ToPagedData(pageInfo);
        }

        public async Task<IPagedData<ProfessionalDto>> GetAllWithDetailsAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            var page = await GetAllAsync(pageInfo, query, cancellationToken);
            var dtos = page.Data.ToList();

            await EnrichWithLicensesAndInsurancesAsync(dtos, cancellationToken);

            return dtos.ToPagedData(pageInfo);
        }

        private async Task EnrichWithLicensesAndInsurancesAsync(List<ProfessionalDto> professionals, CancellationToken cancellationToken)
        {
            var professionalIds = professionals.Select(d => d.Id).ToList();

            if (professionalIds.Count == 0)
            {
                return;
            }

            var licensesByProfessional = await _licenseService.GetAllByProfessionalIdsAsync(professionalIds, ProfessionalType.FogTransporter, cancellationToken);
            var insurancesByProfessional = await _insuranceService.GetAllByProfessionalIdsAsync(professionalIds, cancellationToken);

            foreach (var dto in professionals)
            {
                dto.LicensesAndInsurances = licensesByProfessional[dto.Id]
                    .Select(l => new ProfessionalLicenseOrInsuranceRowDto
                    {
                        LicenseType = l.LicenseType.Name,
                        Number = l.LicenseNumber,
                        ExpirationDate = l.ExpirationDate,
                        ExpirationType = l.ExpirationType
                    })
                    .Concat(insurancesByProfessional[dto.Id].Select(i => new ProfessionalLicenseOrInsuranceRowDto
                    {
                        LicenseType = "Insurance Policy",
                        Number = i.InsuranceNumber,
                        ExpirationDate = i.ExpirationDate,
                        ExpirationType = i.ExpirationType
                    }))
                    .ToList();
            }
        }
    }
}
