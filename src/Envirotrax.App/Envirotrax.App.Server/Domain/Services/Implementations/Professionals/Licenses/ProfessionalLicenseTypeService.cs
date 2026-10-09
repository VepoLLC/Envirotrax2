
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common.Data;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Licenses;

public class ProfessionalLicenseTypeService(IMapper mapper, IProfessionalLicenseTypeRepository repository) : IProfessionalLicenseTypeService
{
    public async Task<IEnumerable<ProfessionalLicenseTypeDto>> GetAllAsync(Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalLicenseType, ProfessionalLicenseTypeDto>(mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalLicenseType, ProfessionalLicenseTypeDto>(mapper);

        var types = await repository.GetAllAsync(query, cancellationToken);
        return types.Select(mapper.Map<ProfessionalLicenseTypeDto>);
    }

    public async Task EnsureLicenseScopeAsync(int? licenseTypeId, LicenseScope licenseScope)
    {
        var licenseType = await repository.GetAsync(licenseTypeId.GetValueOrDefault(), default);

        if (licenseType == null || licenseType.LicenseScope != licenseScope)
        {
            throw new AppValidationException("The selected license type can't be registered here.");
        }
    }
}