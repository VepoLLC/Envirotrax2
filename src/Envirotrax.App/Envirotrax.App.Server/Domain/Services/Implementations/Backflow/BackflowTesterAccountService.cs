
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowTesterAccountService : Service<ProfessionalUser, BackflowTesterAccountDto>, IBackflowTesterAccountService
{
    private readonly IProfessionalUserRepository _repository;

    public BackflowTesterAccountService(IMapper mapper, IProfessionalUserRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<IPagedData<BackflowTesterAccountDto>> SearchForAdminAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<ProfessionalUser, BackflowTesterAccountDto>(Mapper);
        query.Sort = query.ConvertSortProperties<ProfessionalUser, BackflowTesterAccountDto>(Mapper);

        var accounts = await _repository.SearchAccountsAsync(
            pageInfo,
            query,
            licenseNumber,
            insuranceNumber,
            license => license.ProfessionalType == ProfessionalType.Bpat && !license.LicenseType!.IsFireLicense,
            proUser => proUser.IsBackflowTester,
            cancellationToken);

        return accounts
            .Select(a =>
            {
                var dto = MapToDto(a)!;

                dto.IsSubAccount = a.Professional?.ParentId != null;

                return dto;
            })
            .ToPagedData(pageInfo);
    }
}
