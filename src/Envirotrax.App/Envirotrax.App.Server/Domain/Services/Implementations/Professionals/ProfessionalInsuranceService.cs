
using System.ComponentModel.DataAnnotations;
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
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".pdf"];

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
            else if (localTime.AddDays(-30) <= dto.ExpirationDate)
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

    public async Task<ProfessionalInsuranceDto> AddAsync(Stream fileStream, string originalFileName, ProfessionalInsuranceDto insurance)
    {
        var fileExtension = Path.GetExtension(originalFileName).ToLower();
        if (!AllowedFileExtensions.Contains(fileExtension))
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");

        insurance.FilePath = $"professionals/{_authService.ProfessionalId}/insurances/{Guid.NewGuid()}{fileExtension}";

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var added = await AddAsync(insurance);
            await _fileStorageService.UploadAsync(insurance.FilePath, fileStream);

            scope.Complete();
            return added;
        }
    }

    public async Task<ProfessionalInsuranceDto> AddForProfessionalAsync(int professionalId, Stream fileStream, string originalFileName, ProfessionalInsuranceDto dto)
    {
        var fileExtension = Path.GetExtension(originalFileName).ToLower();
        if (!AllowedFileExtensions.Contains(fileExtension))
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");

        dto.FilePath = $"professionals/{professionalId}/insurances/{Guid.NewGuid()}{fileExtension}";

        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var added = await Repository.AddAsync(model);
        await _fileStorageService.UploadAsync(dto.FilePath, fileStream);
        scope.Complete();

        return MapToDto(added)!;
    }

    public async Task<ProfessionalInsuranceDto> UpdateForProfessionalAsync(int professionalId, ProfessionalInsuranceDto dto)
    {
        var existing = await Repository.GetAsync(dto.Id, CancellationToken.None);
        dto.FilePath = existing?.FilePath;

        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;
        var updated = await Repository.UpdateAsync(model);
        return MapToDto(updated)!;
    }

    public async Task<ProfessionalInsuranceDto> UpdateForProfessionalAsync(int professionalId, Stream fileStream, string originalFileName, ProfessionalInsuranceDto dto)
    {
        var existing = await Repository.GetAsync(dto.Id, CancellationToken.None);
        var oldFilePath = existing?.FilePath;

        var fileExtension = Path.GetExtension(originalFileName).ToLower();
        if (!AllowedFileExtensions.Contains(fileExtension))
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");

        dto.FilePath = $"professionals/{professionalId}/insurances/{Guid.NewGuid()}{fileExtension}";

        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var updated = await Repository.UpdateAsync(model);
        await _fileStorageService.UploadAsync(dto.FilePath, fileStream);
        if (oldFilePath != null)
            await _fileStorageService.DeleteAsync(oldFilePath);
        scope.Complete();

        return MapToDto(updated)!;
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
}
