
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowGaugeService : Service<BackflowGauge, BackflowGaugeDto>, IBackflowGaugeService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".pdf"];

    private readonly IFileStorageService _fileStorageService;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IAuthService _authService;

    public BackflowGaugeService(
        IMapper mapper,
        IBackflowGaugeRepository repository,
        IFileStorageService fileStorageService,
        ITimeZoneHelperService timeZoneHelper,
        IAuthService authService)
        : base(mapper, repository)
    {
        _fileStorageService = fileStorageService;
        _timeZoneHelper = timeZoneHelper;
        _authService = authService;
    }

    protected override BackflowGaugeDto? MapToDto(BackflowGauge? model)
    {
        var dto = base.MapToDto(model);

        if (dto?.LastCalibrationDate != null)
        {
            var localTime = _timeZoneHelper.GetUserLocalTime();
            var expirationDate = dto.LastCalibrationDate.Value.AddYears(1);

            if (localTime >= expirationDate)
                dto.ExpirationType = GaugeExpirationType.Expired;
            else if (localTime.AddDays(30) >= expirationDate)
                dto.ExpirationType = GaugeExpirationType.AboutToExpire;
        }

        return dto;
    }

    public async Task<BackflowGaugeDto> AddWithFileAsync(Stream fileStream, string originalFileName, BackflowGaugeDto dto)
    {
        var fileExtension = Path.GetExtension(originalFileName).ToLower();
        if (!AllowedFileExtensions.Contains(fileExtension))
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");

        dto.FilePath = $"professionals/{_authService.ProfessionalId}/gauges/{Guid.NewGuid()}{fileExtension}";

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var added = await AddAsync(dto);
        await _fileStorageService.UploadAsync(dto.FilePath, fileStream);
        scope.Complete();
        return added;
    }
}
