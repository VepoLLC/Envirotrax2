
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalInsuranceService : Service<ProfessionalInsurance, ProfessionalInsuranceDto>, IProfessionalInsuranceService
{
    private readonly IProfessionalInsuranceRepository _insuranceRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IAuthService _authService;

    public ProfessionalInsuranceService(
        IMapper mapper,
        IProfessionalInsuranceRepository repository,
        IFileStorageService fileStorageService,
        ITimeZoneHelperService timeZoneHelper,
        IAuthService authService)
        : base(mapper, repository)
    {
        _insuranceRepository = repository;
        _fileStorageService = fileStorageService;
        _timeZoneHelper = timeZoneHelper;
        _authService = authService;
    }

    protected override ProfessionalInsuranceDto? MapToDto(ProfessionalInsurance? model)
    {
        var dto = base.MapToDto(model);

        if (dto != null)
        {
            var localTime = _timeZoneHelper.GetUserLocalTime();

            if (localTime > dto.ExpirationDate)
            {
                dto.ExpirationType = ExpirationType.Expired;
            }
            else if (localTime.AddDays(30) >= dto.ExpirationDate)
            {
                dto.ExpirationType = ExpirationType.AboutToExpire;
            }
        }

        return dto;
    }

    public async Task<IPagedData<ProfessionalInsuranceDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalInsurance, ProfessionalInsuranceDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalInsurance, ProfessionalInsuranceDto>(Mapper);

        var items = await _insuranceRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);

        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }

    public async Task<ProfessionalInsuranceDto> AddAsync(Stream fileStream, string originalFileName, ProfessionalInsuranceDto dto)
    {
        var professionalId = dto.Professional?.Id ?? _authService.ProfessionalId;
        var fileExtension = Path.GetExtension(originalFileName);
        dto.FilePath = $"professionals/{professionalId}/insurances/{Guid.NewGuid()}{fileExtension}";

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var added = await AddAsync(dto);
        await _fileStorageService.UploadAsync(dto.FilePath, fileStream);
        scope.Complete();

        return added;
    }

    public override async Task<ProfessionalInsuranceDto?> DeleteAsync(int id)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var deleted = await base.DeleteAsync(id);

            if (deleted != null)
            {
                await _fileStorageService.DeleteAsync(deleted.FilePath!);
                scope.Complete();
            }

            return deleted;
        }
    }

    public async Task<Uri?> GenerateFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        var insurance = await _insuranceRepository.GetNoIncludesAsync(id, cancellationToken);

        if (insurance != null && !string.IsNullOrWhiteSpace(insurance.FilePath))
        {
            return await _fileStorageService.GenerateSasUrlAsync(insurance.FilePath);
        }

        return null;
    }
}
