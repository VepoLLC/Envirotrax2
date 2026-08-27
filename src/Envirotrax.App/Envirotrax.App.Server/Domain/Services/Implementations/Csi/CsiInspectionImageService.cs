using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionImageService : ICsiInspectionImageService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".tiff"];

    private const int MaxImagesPerInspection = 24;

    private readonly IMapper _mapper;
    private readonly ICsiInspectionImageRepository _imageRepository;
    private readonly ICsiInspectionRepository _inspectionRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthService _authService;

    public CsiInspectionImageService(
        IMapper mapper,
        ICsiInspectionImageRepository imageRepository,
        ICsiInspectionRepository inspectionRepository,
        IFileStorageService fileStorageService,
        IAuthService authService)
    {
        _mapper = mapper;
        _imageRepository = imageRepository;
        _inspectionRepository = inspectionRepository;
        _fileStorageService = fileStorageService;
        _authService = authService;
    }

    public async Task<List<CsiInspectionImageDto>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        var images = await _imageRepository.GetByInspectionAsync(inspectionId, cancellationToken);

        var dtos = new List<CsiInspectionImageDto>(images.Count);

        if (images.Count > 0)
        {
            var delegationKey = await _fileStorageService.GetUserDelegationKeyAsync();
            foreach (var img in images)
            {
                var dto = _mapper.Map<CsiInspectionImageDto>(img);
                dto.Url = (await _fileStorageService.GenerateSasUrlAsync(delegationKey, img.FilePath)).ToString();
                dtos.Add(dto);
            }
        }

        return dtos;
    }

    public async Task<CsiInspectionImageDto> AddImageAsync(
        int inspectionId,
        string? description,
        Stream imageStream,
        string fileName,
        CancellationToken cancellationToken)
    {
        var inspection = await _inspectionRepository.GetAsync(inspectionId, cancellationToken)
            ?? throw new ValidationException("Inspection not found.");

        var isAdmin = _authService.HasScope(ScopeDefinitions.AdminInternal);
        var professionalId = isAdmin ? inspection.ProfessionalId : _authService.ProfessionalId;

        if (!isAdmin && inspection.ProfessionalId != professionalId)
            throw new ValidationException("Access denied.");

        var count = await _imageRepository.GetCountByInspectionAsync(inspectionId, cancellationToken);
        if (count >= MaxImagesPerInspection)
        {
            throw new ValidationException($"The maximum number of images allowed is {MaxImagesPerInspection}.");
        }

        var ext = ValidateAndGetExtension(fileName);
        var filePath = $"professionals/{professionalId}/csi-inspections/{inspectionId}/{Guid.NewGuid()}{ext}";

        var entity = new CsiInspectionImage
        {
            InspectionId = inspectionId,
            Description = description,
            FilePath = filePath,
            WaterSupplierId = inspection.WaterSupplierId,
            ProfessionalId = professionalId
        };

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var saved = await _imageRepository.AddAsync(entity);
        await _fileStorageService.UploadAsync(filePath, imageStream);
        scope.Complete();

        var dto = _mapper.Map<CsiInspectionImageDto>(saved);
        dto.Url = (await _fileStorageService.GenerateSasUrlAsync(filePath)).ToString();
        return dto;
    }

    public async Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetAsync(imageId, cancellationToken);
        if (image == null)
        {
            return false;
        }

        if (!_authService.HasScope(ScopeDefinitions.AdminInternal) && image.ProfessionalId != _authService.ProfessionalId)
            return false;

        var filePath = image.FilePath;

        await _imageRepository.DeleteAsync(imageId);
        await _fileStorageService.DeleteAsync(filePath);

        return true;
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
