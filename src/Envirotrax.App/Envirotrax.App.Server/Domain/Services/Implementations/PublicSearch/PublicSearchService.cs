
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.PublicSearch;
using Envirotrax.App.Server.Data.Repositories.Definitions.PublicSearch;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.PublicSearch;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.PublicSearch;
using UserDelegationKey = Azure.Storage.Blobs.Models.UserDelegationKey;

namespace Envirotrax.App.Server.Domain.Services.Implementations.PublicSearch;

public class PublicSearchService : IPublicSearchService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IMapper _mapper;
    private readonly IPublicSearchRepository _publicSearchRepository;
    private readonly IBackflowTestService _backflowTestService;
    private readonly ICsiInspectionService _csiInspectionService;
    private readonly IFileStorageService _fileStorageService;

    public PublicSearchService(
        IMapper mapper,
        IPublicSearchRepository publicSearchRepository,
        IBackflowTestService backflowTestService,
        ICsiInspectionService csiInspectionService,
        IFileStorageService fileStorageService)
    {
        _mapper = mapper;
        _publicSearchRepository = publicSearchRepository;
        _backflowTestService = backflowTestService;
        _csiInspectionService = csiInspectionService;
        _fileStorageService = fileStorageService;
    }

    public async Task<PublicSearchWaterSuppliersDto> GetWaterSuppliersAsync(string? domain, CancellationToken cancellationToken)
    {
        var normalizedDomain = NormalizeText(domain)?.ToLowerInvariant();
        var suppliers = (await _publicSearchRepository.GetWaterSuppliersAsync(normalizedDomain, cancellationToken)).ToList();

        if (normalizedDomain == null)
        {
            return new PublicSearchWaterSuppliersDto
            {
                Suppliers = _mapper.Map<List<PublicSearchWaterSupplierDto>>(suppliers)
            };
        }

        var selectedSupplier = suppliers.OrderBy(supplier => supplier.Id).FirstOrDefault();
        var visibleSuppliers = selectedSupplier == null ? [] : new List<PublicSearchWaterSupplier> { selectedSupplier };

        return new PublicSearchWaterSuppliersDto
        {
            Suppliers = _mapper.Map<List<PublicSearchWaterSupplierDto>>(visibleSuppliers),
            SelectedWaterSupplierId = selectedSupplier?.Id
        };
    }

    public async Task<IPagedData<PublicBackflowTestResultDto>> SearchBackflowTestsAsync(
        PublicSearchCriteriaDto criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken)
    {
        var searchCriteria = BuildCriteria(criteria);

        if (searchCriteria == null)
        {
            return Enumerable.Empty<PublicBackflowTestResultDto>().ToPagedData(pageInfo);
        }

        LimitPageSize(pageInfo);

        var results = await _publicSearchRepository.SearchBackflowTestsAsync(searchCriteria, pageInfo, cancellationToken);

        return _mapper
            .Map<IEnumerable<PublicBackflowTestResult>, IEnumerable<PublicBackflowTestResultDto>>(results)
            .ToPagedData(pageInfo);
    }

    public async Task<IPagedData<PublicCsiInspectionResultDto>> SearchCsiInspectionsAsync(
        PublicSearchCriteriaDto criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken)
    {
        var searchCriteria = BuildCriteria(criteria);

        if (searchCriteria == null)
        {
            return Enumerable.Empty<PublicCsiInspectionResultDto>().ToPagedData(pageInfo);
        }

        LimitPageSize(pageInfo);

        var results = await _publicSearchRepository.SearchCsiInspectionsAsync(searchCriteria, pageInfo, cancellationToken);

        return _mapper
            .Map<IEnumerable<PublicCsiInspectionResult>, IEnumerable<PublicCsiInspectionResultDto>>(results)
            .ToPagedData(pageInfo);
    }

    public async Task<PublicBackflowTestDetailsDto?> GetBackflowTestAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _publicSearchRepository.GetBackflowTestAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        var testDto = _mapper.Map<BackflowTestDto>(test);
        var details = _mapper.Map<PublicBackflowTestDetailsDto>(testDto);
        var settings = await _publicSearchRepository.GetBackflowSettingsAsync(test.WaterSupplierId, cancellationToken);

        details.ShowWaterMeterNumber = settings?.ShowWaterMeterNumber ?? false;
        details.ShowRainSensor = settings?.ShowRainSensor ?? false;
        details.ShowOSSF = settings?.ShowOSSF ?? false;
        details.ShowPermitNumber = settings?.ShowPermitNumber ?? false;

        await PopulateImageUrlsAsync(details, testDto);

        return details;
    }

    public async Task<byte[]?> GetBackflowTestPdfAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _publicSearchRepository.GetBackflowTestAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        return await _backflowTestService.GeneratePdfAsync(_mapper.Map<BackflowTestDto>(test));
    }

    public async Task<PublicCsiInspectionDetailsDto?> GetCsiInspectionAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _publicSearchRepository.GetCsiInspectionAsync(id, cancellationToken);

        if (inspection == null)
        {
            return null;
        }

        var inspectionDto = _mapper.Map<CsiInspectionDto>(inspection);

        return _mapper.Map<PublicCsiInspectionDetailsDto>(inspectionDto);
    }

    public async Task<List<CsiInspectionAssemblyDto>> GetCsiInspectionAssembliesAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _publicSearchRepository.GetCsiInspectionAsync(id, cancellationToken);

        if (inspection == null)
        {
            return [];
        }

        var assemblies = await _publicSearchRepository.GetCsiInspectionAssembliesAsync(
            inspection.WaterSupplierId, inspection.Id, cancellationToken);

        return _mapper.Map<List<CsiInspectionAssemblyDto>>(assemblies);
    }

    public async Task<List<CsiInspectionImageDto>> GetCsiInspectionImagesAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _publicSearchRepository.GetCsiInspectionAsync(id, cancellationToken);

        if (inspection == null)
        {
            return [];
        }

        var images = await _publicSearchRepository.GetCsiInspectionImagesAsync(
            inspection.WaterSupplierId, inspection.Id, cancellationToken);

        if (images.Count == 0)
        {
            return [];
        }

        var delegationKey = await _fileStorageService.GetUserDelegationKeyAsync();
        var imageDtos = new List<CsiInspectionImageDto>();

        foreach (var image in images)
        {
            imageDtos.Add(new CsiInspectionImageDto
            {
                Id = image.Id,
                InspectionId = image.InspectionId,
                Description = image.Description,
                Url = await GetImageUrlAsync(delegationKey, image.FilePath)
            });
        }

        return imageDtos;
    }

    public async Task<byte[]?> GetCsiInspectionPdfAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _publicSearchRepository.GetCsiInspectionAsync(id, cancellationToken);

        if (inspection == null)
        {
            return null;
        }

        return await _csiInspectionService.GeneratePdfAsync(_mapper.Map<CsiInspectionDto>(inspection));
    }

    private async Task PopulateImageUrlsAsync(PublicBackflowTestDetailsDto details, BackflowTestDto test)
    {
        var paths = new[]
        {
            test.AssemblyImagePath,
            test.SerialNumberImagePath,
            test.BypassAssemblyImagePath,
            test.BypassSerialNumberImagePath,
            test.AirGapImagePath
        };

        if (paths.All(string.IsNullOrWhiteSpace))
        {
            return;
        }

        var delegationKey = await _fileStorageService.GetUserDelegationKeyAsync();

        details.AssemblyImageUrl = await GetImageUrlAsync(delegationKey, test.AssemblyImagePath);
        details.SerialNumberImageUrl = await GetImageUrlAsync(delegationKey, test.SerialNumberImagePath);
        details.BypassAssemblyImageUrl = await GetImageUrlAsync(delegationKey, test.BypassAssemblyImagePath);
        details.BypassSerialNumberImageUrl = await GetImageUrlAsync(delegationKey, test.BypassSerialNumberImagePath);
        details.AirGapImageUrl = await GetImageUrlAsync(delegationKey, test.AirGapImagePath);
    }

    private async Task<string?> GetImageUrlAsync(UserDelegationKey delegationKey, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var url = await _fileStorageService.GenerateSasUrlAsync(delegationKey, path);

        return url.ToString();
    }

    private PublicSearchCriteria? BuildCriteria(PublicSearchCriteriaDto dto)
    {
        var criteria = _mapper.Map<PublicSearchCriteria>(dto);

        criteria.PropertyBusinessName = NormalizeText(criteria.PropertyBusinessName);
        criteria.PropertyStreetNumber = NormalizeText(criteria.PropertyStreetNumber);
        criteria.PropertyStreetName = NormalizeText(criteria.PropertyStreetName);
        criteria.PropertyNumber = NormalizeText(criteria.PropertyNumber);

        var hasAddressCriteria = criteria.PropertyBusinessName != null
            || criteria.PropertyStreetNumber != null
            || criteria.PropertyStreetName != null
            || criteria.PropertyNumber != null;

        if (criteria.WaterSupplierId <= 0 || !hasAddressCriteria)
        {
            return null;
        }

        return criteria;
    }

    private static string? NormalizeText(string? value)
    {
        var trimmed = value?.Trim();

        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static void LimitPageSize(PageInfo pageInfo)
    {
        pageInfo.PageSize = pageInfo.PageSize > 0
            ? Math.Min(pageInfo.PageSize, MaxPageSize)
            : DefaultPageSize;
    }
}
