using AutoMapper;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Crm;
using Envirotrax.App.Server.Data.Repositories.Definitions.Crm;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Crm;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Crm;

public class CompanyService : Service<Company, CompanyDto>, ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IWaterSupplierRepository _waterSupplierRepository;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly ITimeZoneHelperService _timeZoneHelper;

    // Read once per request, so every company in a list is compared with the same day.
    private DateTime? _today;

    private bool IsAdmin => _authService.HasScope(ScopeDefinitions.AdminInternal);

    public CompanyService(
        IMapper mapper,
        ICompanyRepository repository,
        IWaterSupplierRepository waterSupplierRepository,
        IUserService userService,
        IAuthService authService,
        ITimeZoneHelperService timeZoneHelper)
        : base(mapper, repository)
    {
        _companyRepository = repository;
        _waterSupplierRepository = waterSupplierRepository;
        _userService = userService;
        _authService = authService;
        _timeZoneHelper = timeZoneHelper;
    }

    public override async Task<CompanyDto> AddAsync(CompanyDto dto)
    {
        var company = MapToModel(dto)!;

        company.Id = 0;
        company.WaterSupplierId = await GetCrmWaterSupplierIdAsync(CancellationToken.None);

        var added = await _companyRepository.AddAsync(company);

        return MapToDto(added)!;
    }

    public override async Task<CompanyDto> UpdateAsync(CompanyDto dto)
    {
        var company = MapToModel(dto)!;
        var updated = await _companyRepository.UpdateCompanyAsync(company, CancellationToken.None);

        if (updated == null)
        {
            throw new InvalidOperationException($"Company {dto.Id} not found.");
        }

        return MapToDto(updated)!;
    }

    public async Task<IPagedData<WaterSupplierUserDto>> GetCrmUsersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var waterSupplierId = await GetCrmWaterSupplierIdAsync(cancellationToken);

        return await _userService.GetAllForWaterSupplierAsync(waterSupplierId, pageInfo, query, cancellationToken);
    }

    // Every company this service returns carries its next contact status, whichever method returns it.
    protected override CompanyDto? MapToDto(Company? model)
    {
        var dto = base.MapToDto(model);

        if (dto != null)
        {
            _today ??= _timeZoneHelper.GetUserLocalTime().Date;
            dto.NextContactDateStatus = ComputeNextContactDateStatus(dto.NextContactDate, _today.Value);
        }

        return dto;
    }

    private static NextContactDateStatus ComputeNextContactDateStatus(DateTime? nextContactDate, DateTime today)
    {
        if (!nextContactDate.HasValue)
        {
            return NextContactDateStatus.None;
        }

        if (nextContactDate.Value.Date <= today)
        {
            return NextContactDateStatus.Due;
        }

        return NextContactDateStatus.Upcoming;
    }

    // Envirotrax.Admin users are users of the Vepo LLC water supplier, and their sign-in carries no water supplier,
    // so the CRM records they keep belong to Vepo LLC. A water supplier user keeps the records of their own supplier.
    private async Task<int> GetCrmWaterSupplierIdAsync(CancellationToken cancellationToken)
    {
        if (IsAdmin)
        {
            return await _waterSupplierRepository.GetEnvirotraxAdminSupplierIdAsync(cancellationToken);
        }

        return _authService.WaterSupplierId;
    }
}
