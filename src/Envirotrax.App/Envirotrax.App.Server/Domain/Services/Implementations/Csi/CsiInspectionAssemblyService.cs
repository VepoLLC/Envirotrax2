using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

// Works the way V1's csi_inspection_submit.aspx did: while the form is open, rows are saved under its
// SubmissionId, and submitting the inspection links them (and the tests added on the form) to it.
// An added assembly is a BackflowTest row, because Envirotrax stores assemblies and their tests together.
public class CsiInspectionAssemblyService : ICsiInspectionAssemblyService
{
    private readonly IMapper _mapper;
    private readonly ICsiInspectionAssemblyRepository _repository;
    private readonly IBackflowTestRepository _testRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IBackflowTestService _testService;
    private readonly IAuthService _authService;

    public CsiInspectionAssemblyService(
        IMapper mapper,
        ICsiInspectionAssemblyRepository repository,
        IBackflowTestRepository testRepository,
        ISiteRepository siteRepository,
        IBackflowTestService testService,
        IAuthService authService)
    {
        _mapper = mapper;
        _repository = repository;
        _testRepository = testRepository;
        _siteRepository = siteRepository;
        _testService = testService;
        _authService = authService;
    }

    public async Task<List<CsiInspectionAssemblyDto>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        var assemblies = await _repository.GetByInspectionAsync(inspectionId, cancellationToken);

        return _mapper.Map<List<CsiInspectionAssemblyDto>>(assemblies);
    }

    public Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        return _repository.GetCountByInspectionAsync(inspectionId, cancellationToken);
    }

    // V1 initializeAssemblies: when the form opens, every current test at the site that this submission
    // does not list yet gets a row.
    public async Task<List<CsiInspectionAssemblyDto>> InitializeAsync(int siteId, string submissionId, CancellationToken cancellationToken)
    {
        var assemblies = await _repository.GetBySubmissionAsync(submissionId, cancellationToken);
        var listedTestIds = assemblies.Select(assembly => assembly.TestId).ToHashSet();

        var tests = await _testRepository.GetCurrentBySiteAsync(siteId, cancellationToken);

        foreach (var test in tests.Where(test => !listedTestIds.Contains(test.Id)))
        {
            await _repository.AddAsync(BuildAssembly(test, submissionId));
        }

        var initialized = await _repository.GetBySubmissionAsync(submissionId, cancellationToken);

        return _mapper.Map<List<CsiInspectionAssemblyDto>>(initialized);
    }

    // V1 btnAddAssemblyOK_Click: the new test has no site until the inspection is submitted.
    public async Task<CsiInspectionAssemblyDto> AddAsync(CsiInspectionAssemblyRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var site = await _siteRepository.GetNoIncludesAsync(request.SiteId, cancellationToken)
            ?? throw new AppValidationException("The location of this inspection was not found.");

        var test = new BackflowTest
        {
            WaterSupplierId = site.WaterSupplierId,
            ProfessionalId = _authService.ProfessionalId,
            InspectorId = _authService.UserId,
            SubmissionId = request.SubmissionId,
            DeviceType = request.DeviceType,
            Manufacturer = request.Manufacturer,
            Model = request.Model,
            Size = request.Size,
            SerialNumber = request.SerialNumber?.Trim(),
            Manufacturer2 = request.Manufacturer2,
            Model2 = request.Model2,
            Size2 = request.Size2,
            SerialNumber2 = request.SerialNumber2?.Trim(),
            HazardType = request.HazardType,
            HazardTypeOtherDescription = request.HazardTypeOtherDescription?.Trim(),
            LocationDescription = request.LocationDescription?.Trim(),
            Comments = request.Comments?.Trim(),
            ReasonForTest = BackflowReasonForTest.AnnualTest,
            IsCurrent = true,
            NeedsValidation = true
        };

        // The renewal rules read the property type and aux water supply from the site, which the new
        // test is not linked to yet.
        test.RenewalRequired = await _testService.IsRenewalRequiredAsync(new BackflowTest
        {
            WaterSupplierId = site.WaterSupplierId,
            DeviceType = test.DeviceType,
            HazardType = test.HazardType,
            PropertyType = (int)site.PropertyType,
            Site = site
        }, cancellationToken);

        var assembly = BuildAssembly(test, request.SubmissionId);
        assembly.Test = test;

        await _repository.AddAsync(assembly);

        return _mapper.Map<CsiInspectionAssemblyDto>(assembly);
    }

    public Task<bool> DeleteAsync(int id, string submissionId, CancellationToken cancellationToken)
    {
        return _repository.DeleteForSubmissionAsync(id, submissionId, cancellationToken);
    }

    public Task UpdateVisuallyIdentifiedAsync(CsiInspectionVisuallyIdentifiedRequest request, CancellationToken cancellationToken)
    {
        return _repository.UpdateVisuallyIdentifiedAsync(request.SubmissionId, request.VisuallyIdentifiedIds, cancellationToken);
    }

    // V1 setBackflowRecords, run when the inspection is submitted or edited.
    public async Task LinkToInspectionAsync(CsiInspection inspection, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(inspection.SubmissionId))
        {
            return;
        }

        await _repository.LinkToInspectionAsync(inspection.Id, inspection.SubmissionId, cancellationToken);
        await _testRepository.ApplyCsiInspectionAsync(inspection, cancellationToken);
    }

    // V1 checkout: the tests added on the form and the inspection's rows are paid with the inspection.
    public async Task MarkPaidAsync(IEnumerable<CsiInspection> inspections, string transactionId, DateTime transactionDate, CancellationToken cancellationToken)
    {
        foreach (var inspection in inspections.Where(inspection => !string.IsNullOrEmpty(inspection.SubmissionId)))
        {
            await _testRepository.MarkCsiInspectionTestsPaidAsync(inspection.SiteId, inspection.SubmissionId!, transactionId, transactionDate, cancellationToken);
            await _repository.MarkPaidAsync(inspection.Id, inspection.SubmissionId!, transactionId, cancellationToken);
        }
    }

    public Task DeleteByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        return _repository.DeleteByInspectionAsync(inspectionId, cancellationToken);
    }

    private static void ValidateRequest(CsiInspectionAssemblyRequest request)
    {
        if (!Enum.GetNames<BackflowDeviceType>().Contains(request.DeviceType))
        {
            throw new AppValidationException("Please select a valid Backflow Method.");
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

        if (request.HazardType == "Other" && string.IsNullOrWhiteSpace(request.HazardTypeOtherDescription))
        {
            throw new AppValidationException("Other Description is required when the Hazard Type is Other.");
        }
    }

    // V1 addVisuallyIdentifiedAssemblyFromTest: the row is a snapshot of the test.
    private static CsiInspectionVisuallyIdentifiedAssembly BuildAssembly(BackflowTest test, string submissionId)
    {
        var hasBypass = BackflowDeviceTypes.HasBypassAssembly(test.DeviceType);

        return new CsiInspectionVisuallyIdentifiedAssembly
        {
            WaterSupplierId = test.WaterSupplierId,
            SubmissionId = submissionId,
            TestId = test.Id > 0 ? test.Id : null,
            DeviceType = test.DeviceType,
            AssemblyDescription = BuildDescription(test.Manufacturer, test.Model, test.Size, test.DeviceType),
            SerialNumber = test.SerialNumber,
            AssemblyDescription2 = hasBypass ? BuildDescription(test.Manufacturer2, test.Model2, test.Size2, test.DeviceType) : null,
            SerialNumber2 = hasBypass ? test.SerialNumber2 : null,
            HazardType = test.HazardType,
            HazardTypeOtherDescription = test.HazardTypeOtherDescription,
            LocationDescription = test.LocationDescription,
            IsCurrent = test.IsCurrent,
            TestResult = test.TestResult,
            OutOfService = test.OutOfService,
            TestDate = test.TestDate,
            ExpirationDate = test.ExpirationDate,
            TransactionId = test.TransactionId,
            CreatedTime = DateTime.UtcNow
        };
    }

    // V1 BackflowTest.DeviceDescription.
    private static string BuildDescription(string? manufacturer, string? model, string? size, string? deviceType)
    {
        return $"{manufacturer} {model} {size} - {deviceType}";
    }
}
