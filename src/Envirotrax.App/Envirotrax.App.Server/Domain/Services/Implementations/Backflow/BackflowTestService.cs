using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowTestService : Service<BackflowTest, BackflowTestDto>, IBackflowTestService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".tiff"];

    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthService _authService;

    public BackflowTestService(
        IMapper mapper,
        IBackflowTestRepository repository,
        IProfessionalRepository professionalRepository,
        IProfessionalUserRepository professionalUserRepository,
        IFileStorageService fileStorageService,
        IAuthService authService)
        : base(mapper, repository)
    {
        _professionalRepository = professionalRepository;
        _professionalUserRepository = professionalUserRepository;
        _fileStorageService = fileStorageService;
        _authService = authService;
    }

    private async Task PopulateBpatSnapshotAsync(BackflowTestDto dto)
    {
        if (dto.Professional?.Id is int professionalId)
        {
            var professional = await _professionalRepository.GetAsync(professionalId, CancellationToken.None);
            if (professional != null)
            {
                dto.BpatCompanyName = professional.Name;
                dto.BpatAddress = professional.Address;
                dto.BpatCity = professional.City;
                dto.BpatZip = professional.ZipCode;
                dto.BpatWorkNumber = professional.PhoneNumber;
                if (professional.StateId.HasValue)
                    dto.BpatState = new ReferencedStateDto { Id = professional.StateId.Value };
            }
        }

        if (dto.Bpat?.Id is int bpatId)
        {
            var bpatUser = await _professionalUserRepository.GetAsync(bpatId, CancellationToken.None);
            if (bpatUser != null)
                dto.BpatContactName = bpatUser.ContactName;
        }
    }

    public async Task<BackflowTestDto> SubmitWithImagesAsync(
        BackflowTestDto dto,
        Stream? assemblyStream, string? assemblyFileName,
        Stream? serialStream, string? serialFileName,
        Stream? bypassAssemblyStream, string? bypassAssemblyFileName,
        Stream? bypassSerialStream, string? bypassSerialFileName,
        Stream? airGapStream, string? airGapFileName,
        CancellationToken cancellationToken = default)
    {
        var professionalId = _authService.ProfessionalId;
        dto.Professional = new ReferencedProfessionalDto { Id = professionalId };

        await PopulateBpatSnapshotAsync(dto);

        // Set paths before AddAsync to avoid a second EF update (double-tracking conflict)
        if (assemblyStream != null && assemblyFileName != null)
        {
            dto.AssemblyImagePath = $"professionals/{professionalId}/backflow-tests/assembly/{Guid.NewGuid()}{ValidateAndGetExtension(assemblyFileName)}";
        }
        if (serialStream != null && serialFileName != null)
        {
            dto.SerialNumberImagePath = $"professionals/{professionalId}/backflow-tests/serial-number/{Guid.NewGuid()}{ValidateAndGetExtension(serialFileName)}";
        }
        if (bypassAssemblyStream != null && bypassAssemblyFileName != null)
        {
            dto.BypassAssemblyImagePath = $"professionals/{professionalId}/backflow-tests/bypass-assembly/{Guid.NewGuid()}{ValidateAndGetExtension(bypassAssemblyFileName)}";
        }
        if (bypassSerialStream != null && bypassSerialFileName != null)
        {
            dto.BypassSerialNumberImagePath = $"professionals/{professionalId}/backflow-tests/bypass-serial-number/{Guid.NewGuid()}{ValidateAndGetExtension(bypassSerialFileName)}";
        }
        if (airGapStream != null && airGapFileName != null)
        {
            dto.AirGapImagePath = $"professionals/{professionalId}/backflow-tests/air-gap/{Guid.NewGuid()}{ValidateAndGetExtension(airGapFileName)}";
        }

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var saved = await base.AddAsync(dto);

        if (assemblyStream != null && dto.AssemblyImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.AssemblyImagePath, assemblyStream);
        }
        if (serialStream != null && dto.SerialNumberImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.SerialNumberImagePath, serialStream);
        }
        if (bypassAssemblyStream != null && dto.BypassAssemblyImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.BypassAssemblyImagePath, bypassAssemblyStream);
        }
        if (bypassSerialStream != null && dto.BypassSerialNumberImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.BypassSerialNumberImagePath, bypassSerialStream);
        }
        if (airGapStream != null && dto.AirGapImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.AirGapImagePath, airGapStream);
        }

        scope.Complete();
        return saved;
    }

    private static string ValidateAndGetExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName);

        if (!AllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");
        }

        return ext;
    }
}
