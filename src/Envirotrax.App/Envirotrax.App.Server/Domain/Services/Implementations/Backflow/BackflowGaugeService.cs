
using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowGaugeService : Service<BackflowGauge, BackflowGaugeDto>, IBackflowGaugeService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthService _authService;

    public BackflowGaugeService(
        IMapper mapper,
        IBackflowGaugeRepository repository,
        IFileStorageService fileStorageService,
        IAuthService authService)
        : base(mapper, repository)
    {
        _fileStorageService = fileStorageService;
        _authService = authService;
    }

    public async Task<BackflowGaugeDto> AddWithFileAsync(Stream fileStream, string originalFileName, BackflowGaugeDto dto)
    {
        var fileExtension = Path.GetExtension(originalFileName);
        dto.FilePath = $"professionals/{_authService.ProfessionalId}/gauges/{Guid.NewGuid()}{fileExtension}";

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var added = await AddAsync(dto);
        await _fileStorageService.UploadAsync(dto.FilePath, fileStream);
        scope.Complete();
        return added;
    }
}
