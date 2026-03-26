
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Licenses;

public class ProfessionalUserLicenseService : Service<ProfessionalUserLicense, ProfessionalUserLicenseDto>, IProfessionalUserLicenseService
{
    private readonly IProfessionalUserLicenseRepository _licenseRepository;

    public ProfessionalUserLicenseService(IMapper mapper, IProfessionalUserLicenseRepository repository)
        : base(mapper, repository)
    {
        _licenseRepository = repository;
    }

    public async Task<IPagedData<ProfessionalUserLicenseDto>> GetAllAsync(int userId, PageInfo pageInfo, Query query)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);

        var licenses = await _licenseRepository.GetAllAsync(userId, pageInfo, query);
        var dtoList = licenses.Select(l => MapToDto(l)!);

        return dtoList.ToPagedData(pageInfo);
    }
}
