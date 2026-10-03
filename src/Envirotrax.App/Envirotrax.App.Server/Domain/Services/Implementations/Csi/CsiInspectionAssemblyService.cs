using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

// Envirotrax stores a backflow assembly and its tests as the same BackflowTests rows, so an assembly
// the inspector adds by sight is saved the way V1 saved it: as a placeholder BackflowTest for the site,
// linked from the visually identified assembly row.
public class CsiInspectionAssemblyService : ICsiInspectionAssemblyService
{
    private const string OtherHazardType = "Other";

    private readonly IMapper _mapper;
    private readonly ICsiInspectionAssemblyRepository _repository;
    private readonly ICsiInspectionRepository _inspectionRepository;
    private readonly IBackflowTestRepository _testRepository;
    private readonly IBackflowTestService _testService;
    private readonly IAuthService _authService;

    public CsiInspectionAssemblyService(
        IMapper mapper,
        ICsiInspectionAssemblyRepository repository,
        ICsiInspectionRepository inspectionRepository,
        IBackflowTestRepository testRepository,
        IBackflowTestService testService,
        IAuthService authService)
    {
        _mapper = mapper;
        _repository = repository;
        _inspectionRepository = inspectionRepository;
        _testRepository = testRepository;
        _testService = testService;
        _authService = authService;
    }

    public async Task<List<CsiInspectionAssemblyDto>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        var assemblies = await _repository.GetByInspectionAsync(inspectionId, cancellationToken);

        return assemblies
            .Select(assembly => _mapper.Map<CsiInspectionAssemblyDto>(assembly))
            .ToList();
    }

    public Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        return _repository.GetCountByInspectionAsync(inspectionId, cancellationToken);
    }

    // The unsaved rows a new submission starts from: one per current test at the site.
    public async Task<List<CsiInspectionAssemblyDto>> GetForSiteAsync(int siteId, CancellationToken cancellationToken)
    {
        var tests = await _testRepository.GetCurrentBySiteAsync(siteId, cancellationToken);

        return tests
            .Select(test =>
            {
                var dto = _mapper.Map<CsiInspectionAssemblyDto>(BuildFromTest(test));

                dto.Disapproved = test.Disapproved;
                dto.Rejected = test.Rejected;

                return dto;
            })
            .ToList();
    }

    // Replaces the inspection's list with `requests`. Returns null when the inspection is not the
    // logged-in professional's or has already been paid for.
    public async Task<List<CsiInspectionAssemblyDto>?> SaveForProfessionalAsync(
        int inspectionId,
        List<CsiInspectionAssemblyRequest> requests,
        CancellationToken cancellationToken)
    {
        var inspection = await _inspectionRepository.GetAsync(inspectionId, cancellationToken);

        if (inspection == null || inspection.ProfessionalId != _authService.ProfessionalId || !string.IsNullOrEmpty(inspection.TransactionId))
        {
            return null;
        }

        var saved = await _repository.GetByInspectionAsync(inspectionId, cancellationToken);
        var assemblies = await BuildAssembliesAsync(inspection, saved, requests, cancellationToken);

        await _repository.SaveForInspectionAsync(inspectionId, inspection.SubmissionId, assemblies, cancellationToken);

        return await GetByInspectionAsync(inspectionId, cancellationToken);
    }

    // Removes every row of an inspection that is being deleted, along with the placeholder tests
    // its submission created.
    public Task DeleteForInspectionAsync(int inspectionId, string? submissionId, CancellationToken cancellationToken)
    {
        return _repository.SaveForInspectionAsync(inspectionId, submissionId, [], cancellationToken);
    }

    private async Task<List<CsiInspectionVisuallyIdentifiedAssembly>> BuildAssembliesAsync(
        CsiInspection inspection,
        List<CsiInspectionVisuallyIdentifiedAssembly> saved,
        List<CsiInspectionAssemblyRequest> requests,
        CancellationToken cancellationToken)
    {
        var assemblies = new List<CsiInspectionVisuallyIdentifiedAssembly>();

        var savedIds = saved.Select(assembly => assembly.Id).ToHashSet();

        // A site test that is already linked keeps its saved row, so resending the list is harmless.
        var savedIdsByTestId = saved
            .Where(assembly => assembly.TestId.HasValue)
            .GroupBy(assembly => assembly.TestId!.Value)
            .ToDictionary(group => group.Key, group => group.First().Id);

        foreach (var request in requests)
        {
            if (!request.Id.HasValue && request.TestId.HasValue && savedIdsByTestId.TryGetValue(request.TestId.Value, out var savedId))
            {
                request.Id = savedId;
            }
        }

        foreach (var request in requests.Where(request => request.Id.HasValue).DistinctBy(request => request.Id))
        {
            if (!savedIds.Contains(request.Id!.Value))
            {
                throw new AppValidationException("One of the assemblies is not part of this inspection. Please reload the page and try again.");
            }

            assemblies.Add(new CsiInspectionVisuallyIdentifiedAssembly
            {
                Id = request.Id.Value,
                VisuallyIdentified = request.VisuallyIdentified
            });
        }

        var siteRequests = requests
            .Where(request => !request.Id.HasValue && request.TestId.HasValue)
            .DistinctBy(request => request.TestId)
            .ToList();

        var siteTests = siteRequests.Count > 0
            ? await _testRepository.GetByIdsAsync(siteRequests.Select(request => request.TestId!.Value), cancellationToken)
            : [];

        foreach (var request in siteRequests)
        {
            var test = siteTests.FirstOrDefault(siteTest => siteTest.Id == request.TestId);

            if (test == null || test.SiteId != inspection.SiteId || test.DeletedTime.HasValue)
            {
                throw new AppValidationException("One of the assemblies is not at this location. Please reload the page and try again.");
            }

            assemblies.Add(BuildForInspection(BuildFromTest(test), inspection, request.VisuallyIdentified));
        }

        foreach (var request in requests.Where(request => !request.Id.HasValue && !request.TestId.HasValue))
        {
            ValidateNewAssembly(request);

            var test = await BuildPlaceholderTestAsync(inspection, request, cancellationToken);

            var assembly = BuildForInspection(BuildFromTest(test), inspection, request.VisuallyIdentified);
            assembly.Test = test;

            assemblies.Add(assembly);
        }

        return assemblies;
    }

    // The fields V1 inserted for an assembly added on the inspection form. Every test-date column is the
    // inspection date — the assembly was seen, not tested — so it is due for a real test straight away.
    private async Task<BackflowTest> BuildPlaceholderTestAsync(CsiInspection inspection, CsiInspectionAssemblyRequest request, CancellationToken cancellationToken)
    {
        var isAirGap = request.DeviceType == nameof(BackflowDeviceType.AG);
        var hasBypass = BackflowDeviceTypes.HasBypassAssembly(request.DeviceType);
        var inspectionDate = inspection.InspectionDate;

        var test = new BackflowTest
        {
            WaterSupplierId = inspection.WaterSupplierId,
            SiteId = inspection.SiteId,
            SubmissionId = inspection.SubmissionId,
            ProfessionalId = inspection.ProfessionalId,
            InspectorId = inspection.InspectorId,

            DeviceType = request.DeviceType,
            Manufacturer = isAirGap ? null : request.Manufacturer?.Trim(),
            Model = isAirGap ? null : request.Model?.Trim(),
            Size = isAirGap ? null : request.Size?.Trim(),
            SerialNumber = isAirGap ? null : request.SerialNumber?.Trim(),
            Manufacturer2 = hasBypass ? request.Manufacturer2?.Trim() : null,
            Model2 = hasBypass ? request.Model2?.Trim() : null,
            Size2 = hasBypass ? request.Size2?.Trim() : null,
            SerialNumber2 = hasBypass ? request.SerialNumber2?.Trim() : null,

            HazardType = request.HazardType,
            HazardTypeOtherDescription = request.HazardType == OtherHazardType ? request.HazardTypeOtherDescription?.Trim() : null,
            LocationDescription = request.LocationDescription?.Trim(),
            Comments = request.Comments?.Trim(),

            ReasonForTest = BackflowReasonForTest.AnnualTest,
            IsCurrent = true,
            NeedsValidation = true,

            TestDate = inspectionDate,
            InitialTestDate = inspectionDate,
            RepairTestDate = inspectionDate,
            FinalTestDate = inspectionDate,
            AirGapTestDate = inspectionDate,
            ExpirationDate = inspectionDate,

            AccountNumber = inspection.Site?.AccountNumber,
            PropertyType = (int)inspection.PropertyType,
            PropertyBusinessName = inspection.PropertyBusinessName,
            PropertyStreetNumber = inspection.PropertyStreetNumber,
            PropertyStreetName = inspection.PropertyStreetName,
            PropertyNumber = inspection.PropertyNumber,
            PropertyCity = inspection.PropertyCity,
            PropertyStateId = inspection.PropertyStateId,
            PropertyZip = inspection.PropertyZip,
            MailingCompanyName = inspection.MailingCompanyName,
            MailingContactName = inspection.MailingContactName,
            MailingStreetNumber = inspection.MailingStreetNumber,
            MailingStreetName = inspection.MailingStreetName,
            MailingNumber = inspection.MailingNumber,
            MailingCity = inspection.MailingCity,
            MailingStateId = inspection.MailingStateId,
            MailingZip = inspection.MailingZip,
            MailingPhoneNumber = inspection.MailingPhoneNumber,
            MailingEmailAddress = inspection.MailingEmailAddress
        };

        // The renewal rules read the site's aux-water-supply flag through test.Site. A separate probe
        // carries it so the new test's own Site navigation stays unset and EF never tries to insert a Site.
        var renewalProbe = new BackflowTest
        {
            WaterSupplierId = test.WaterSupplierId,
            DeviceType = test.DeviceType,
            PropertyType = test.PropertyType,
            HazardType = test.HazardType,
            Site = new Site { HasAuxWaterSupply = inspection.Site?.HasAuxWaterSupply ?? false }
        };

        test.RenewalRequired = await _testService.IsRenewalRequiredAsync(renewalProbe, cancellationToken);

        return test;
    }

    private static void ValidateNewAssembly(CsiInspectionAssemblyRequest request)
    {
        if (string.IsNullOrEmpty(request.DeviceType) || !Enum.GetNames<BackflowDeviceType>().Contains(request.DeviceType))
        {
            throw new AppValidationException("Backflow Method is required for each added assembly.");
        }

        var mainIsIncomplete = string.IsNullOrWhiteSpace(request.Manufacturer)
            || string.IsNullOrWhiteSpace(request.Model)
            || string.IsNullOrWhiteSpace(request.Size)
            || string.IsNullOrWhiteSpace(request.SerialNumber);

        if (request.DeviceType != nameof(BackflowDeviceType.AG) && mainIsIncomplete)
        {
            throw new AppValidationException("Main Assembly Manufacturer, Model, Size and Serial Number are required for this backflow method.");
        }

        var bypassIsIncomplete = string.IsNullOrWhiteSpace(request.Manufacturer2)
            || string.IsNullOrWhiteSpace(request.Model2)
            || string.IsNullOrWhiteSpace(request.Size2)
            || string.IsNullOrWhiteSpace(request.SerialNumber2);

        if (BackflowDeviceTypes.HasBypassAssembly(request.DeviceType) && bypassIsIncomplete)
        {
            throw new AppValidationException("Bypass Assembly Manufacturer, Model, Size and Serial Number are required for this backflow method.");
        }

        if (string.IsNullOrWhiteSpace(request.HazardType))
        {
            throw new AppValidationException("Hazard Type is required for each added assembly.");
        }

        if (request.HazardType == OtherHazardType && string.IsNullOrWhiteSpace(request.HazardTypeOtherDescription))
        {
            throw new AppValidationException("Other Description is required when the Hazard Type is Other.");
        }
    }

    private static CsiInspectionVisuallyIdentifiedAssembly BuildForInspection(
        CsiInspectionVisuallyIdentifiedAssembly assembly,
        CsiInspection inspection,
        bool visuallyIdentified)
    {
        assembly.WaterSupplierId = inspection.WaterSupplierId;
        assembly.InspectionId = inspection.Id;
        assembly.SubmissionId = inspection.SubmissionId;
        assembly.VisuallyIdentified = visuallyIdentified;
        assembly.CreatedTime = DateTime.UtcNow;

        return assembly;
    }

    // The row is a snapshot of the test as it was at inspection time, the same columns V1 copied. The
    // test columns are wider than the snapshot's, hence the trimming to fit.
    private static CsiInspectionVisuallyIdentifiedAssembly BuildFromTest(BackflowTest test)
    {
        var hasBypass = BackflowDeviceTypes.HasBypassAssembly(test.DeviceType);

        return new CsiInspectionVisuallyIdentifiedAssembly
        {
            WaterSupplierId = test.WaterSupplierId,
            TestId = test.Id > 0 ? test.Id : null,
            DeviceType = Fit(test.DeviceType, 50),
            AssemblyDescription = Fit(BuildAssemblyDescription(test.Manufacturer, test.Model, test.Size, test.DeviceType), 200),
            SerialNumber = Fit(test.SerialNumber, 50),
            AssemblyDescription2 = hasBypass ? Fit(BuildAssemblyDescription(test.Manufacturer2, test.Model2, test.Size2, test.DeviceType), 200) : null,
            SerialNumber2 = hasBypass ? Fit(test.SerialNumber2, 50) : null,
            HazardType = Fit(test.HazardType, 50),
            HazardTypeOtherDescription = test.HazardTypeOtherDescription,
            LocationDescription = test.LocationDescription,
            IsCurrent = test.IsCurrent,
            TestResult = test.TestResult,
            OutOfService = test.OutOfService,
            TestDate = test.TestDate,
            ExpirationDate = test.ExpirationDate,
            TransactionId = test.TransactionId
        };
    }

    // V1's BackflowTest.DeviceDescription format, so V2 rows read the same as migrated ones.
    private static string BuildAssemblyDescription(string? manufacturer, string? model, string? size, string? deviceType)
    {
        var device = string.Join(" ", new[] { manufacturer, model, size }.Where(part => !string.IsNullOrWhiteSpace(part)));

        return string.IsNullOrEmpty(device) ? deviceType ?? string.Empty : $"{device} - {deviceType}";
    }

    private static string? Fit(string? value, int maxLength)
    {
        return value != null && value.Length > maxLength ? value[..maxLength] : value;
    }
}
