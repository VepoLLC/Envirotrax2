
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectorAccountService : Service<ProfessionalUser, CsiInspectorAccountDto>, ICsiInspectorAccountService
{
    private readonly IProfessionalUserRepository _repository;

    public CsiInspectorAccountService(IMapper mapper, IProfessionalUserRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<IPagedData<CsiInspectorAccountDto>> SearchForAdminAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<ProfessionalUser, CsiInspectorAccountDto>(Mapper);
        query.Sort = query.ConvertSortProperties<ProfessionalUser, CsiInspectorAccountDto>(Mapper);

        var accounts = await _repository.SearchCsiInspectorsAsync(pageInfo, query, licenseNumber, insuranceNumber, cancellationToken);

        return accounts
            .Select(a => MapToDto(a)!)
            .ToPagedData(pageInfo);
    }
}
