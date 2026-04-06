using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
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

        public async Task<CsiInspectorDetailsDto?> GetDetailsAsync(int id, CancellationToken cancellationToken)
        {
            var professional = await _inspectorRepository.GetWithStateAsync(id, cancellationToken);
            if (professional == null)
            {
                return null;
            }
            var waterSuppliers = await _inspectorRepository.GetWaterSuppliersAsync(id, cancellationToken);
            var subAccounts = await _inspectorRepository.GetSubAccountsAsync(id, cancellationToken);
            var licenses = await _inspectorRepository.GetLicensesAsync(id, cancellationToken);


            return new CsiInspectorDetailsDto
            {
                Id = professional.Id,
                Name = professional.Name,
                CompanyEmail = professional.CompanyEmail,
                Address = professional.Address,
                City = professional.City,
                State = professional.StateId.HasValue
                    ? new ReferencedStateDto { Id = professional.StateId.Value, Name = professional.State?.Name, Code = professional.State?.Code }
                    : null,
                ZipCode = professional.ZipCode,
                PhoneNumber = professional.PhoneNumber,
                FaxNumber = professional.FaxNumber,
                CreatedTime = professional.CreatedTime,
                WaterSuppliers = Mapper.Map<List<ProfessionalWaterSupplierDto>>(waterSuppliers),
                SubAccounts = Mapper.Map<List<ProfessionalUserDto>>(subAccounts),
                Licenses = Mapper.Map<List<ProfessionalUserLicenseDto>>(licenses)
            };
        }
    }
}
