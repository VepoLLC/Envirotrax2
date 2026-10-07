using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.Common.Data;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

// The "Assemblies at this Location" rows are saved together with their inspection. An assembly the
// inspector adds is a new BackflowTest at the site, because Envirotrax stores assemblies and their tests
// in the same table.
public class CsiInspectionAssemblyService : ICsiInspectionAssemblyService
{
    private readonly IMapper _mapper;
    private readonly ICsiInspectionAssemblyRepository _repository;
    private readonly IBackflowTestRepository _testRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IBackflowTestService _testService;

    public CsiInspectionAssemblyService(
        IMapper mapper,
        ICsiInspectionAssemblyRepository repository,
        IBackflowTestRepository testRepository,
        ISiteRepository siteRepository,
        IBackflowTestService testService)
    {
        _mapper = mapper;
        _repository = repository;
        _testRepository = testRepository;
        _siteRepository = siteRepository;
        _testService = testService;
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

    // What the form's Assemblies tab starts with (V1 initializeAssemblies): the inspection's saved rows
    // when editing, plus an unsaved row (Id 0) for every current test at the site not listed yet.
    public async Task<List<CsiInspectionAssemblyDto>> GetForFormAsync(int siteId, int? inspectionId, CancellationToken cancellationToken)
    {
        var assemblies = inspectionId.HasValue
            ? await _repository.GetByInspectionAsync(inspectionId.Value, cancellationToken)
            : [];

        var listedTestIds = assemblies.Select(assembly => assembly.TestId).ToHashSet();
        var tests = await _testRepository.GetCurrentBySiteAsync(siteId, cancellationToken);

        var unlisted = tests
            .Where(test => !listedTestIds.Contains(test.Id))
            .Select(BuildAssembly);

        return _mapper.Map<List<CsiInspectionAssemblyDto>>(assemblies.Concat(unlisted));
    }

    // Runs inside the inspection's submit/update transaction. Rows the inspector deleted go (only unpaid
    // ones can be deleted), the checkboxes are saved, and new rows are added for site tests and for the
    // assemblies added on the form.
    public async Task SaveForInspectionAsync(CsiInspection inspection, CreateCsiInspectionDto request, CancellationToken cancellationToken)
    {
        var saved = await _repository.GetByInspectionAsync(inspection.Id, cancellationToken);

        var keptIds = request.Assemblies
            .Where(assembly => assembly.Id.HasValue)
            .Select(assembly => assembly.Id!.Value)
            .ToHashSet();

        var deletedIds = saved
            .Where(assembly => !keptIds.Contains(assembly.Id) && string.IsNullOrEmpty(assembly.TransactionId))
            .Select(assembly => assembly.Id)
            .ToList();

        if (deletedIds.Count > 0)
        {
            await _repository.DeleteFromInspectionAsync(inspection.Id, deletedIds, cancellationToken);
        }

        if (saved.Count > 0)
        {
            var visuallyIdentifiedIds = request.Assemblies
                .Where(assembly => assembly.Id.HasValue && assembly.VisuallyIdentified)
                .Select(assembly => assembly.Id!.Value)
                .ToList();

            await _repository.UpdateVisuallyIdentifiedAsync(inspection.Id, inspection.ProfessionalId, visuallyIdentifiedIds, cancellationToken);
        }

        var added = await BuildSiteTestAssembliesAsync(inspection, saved, request.Assemblies, cancellationToken);
        added.AddRange(await BuildNewAssembliesAsync(inspection, request.NewAssemblies, cancellationToken));

        if (added.Count > 0)
        {
            await _repository.AddRangeAsync(added, cancellationToken);
        }
    }

    // V1 checkout: the inspection's rows and the tests it added are paid with the inspection.
    public async Task MarkPaidAsync(IEnumerable<CsiInspection> inspections, string transactionId, DateTime transactionDate, CancellationToken cancellationToken)
    {
        foreach (var inspection in inspections)
        {
            await _testRepository.MarkCsiInspectionTestsPaidAsync(inspection, transactionId, transactionDate, cancellationToken);
            await _repository.MarkPaidAsync(inspection.Id, inspection.ProfessionalId, transactionId, cancellationToken);
        }
    }

    public Task DeleteByInspectionAsync(int inspectionId, int professionalId, CancellationToken cancellationToken)
    {
        return _repository.DeleteByInspectionAsync(inspectionId, professionalId, cancellationToken);
    }

    // Site tests the inspector kept that the inspection does not list yet. The form listed them as current
    // when it opened (V1 saved the row at that moment), so a test that stopped being current since then is
    // still snapshotted; ids that are not tests at the inspection's site are ignored.
    private async Task<List<CsiInspectionVisuallyIdentifiedAssembly>> BuildSiteTestAssembliesAsync(
        CsiInspection inspection,
        List<CsiInspectionVisuallyIdentifiedAssembly> saved,
        List<CsiInspectionAssemblySelectionDto> selections,
        CancellationToken cancellationToken)
    {
        var listedTestIds = saved.Select(assembly => assembly.TestId).ToHashSet();

        var newSelections = selections
            .Where(selection => !selection.Id.HasValue && selection.TestId.HasValue && !listedTestIds.Contains(selection.TestId))
            .DistinctBy(selection => selection.TestId)
            .ToList();

        if (newSelections.Count == 0)
        {
            return [];
        }

        var tests = await _testRepository.GetByIdsAsync(newSelections.Select(selection => selection.TestId!.Value), cancellationToken);

        var siteTests = tests
            .Where(test => test.SiteId == inspection.SiteId && test.WaterSupplierId == inspection.WaterSupplierId && test.DeletedTime == null)
            .ToDictionary(test => test.Id);

        return newSelections
            .Where(selection => siteTests.ContainsKey(selection.TestId!.Value))
            .Select(selection => LinkToInspection(BuildAssembly(siteTests[selection.TestId!.Value]), inspection, selection.VisuallyIdentified))
            .ToList();
    }

    // V1 btnAddAssemblyOK_Click + setBackflowRecords: a current test at the inspection's site, dated the
    // inspection date because the assembly was seen, not tested.
    private async Task<List<CsiInspectionVisuallyIdentifiedAssembly>> BuildNewAssembliesAsync(
        CsiInspection inspection,
        List<CsiInspectionNewAssemblyDto> newAssemblies,
        CancellationToken cancellationToken)
    {
        if (newAssemblies.Count == 0)
        {
            return [];
        }

        var site = await _siteRepository.GetNoIncludesAsync(inspection.SiteId, cancellationToken)
            ?? throw new AppValidationException("The location of this inspection was not found.");

        var assemblies = new List<CsiInspectionVisuallyIdentifiedAssembly>();

        foreach (var newAssembly in newAssemblies)
        {
            ValidateNewAssembly(newAssembly);

            var test = BuildTest(inspection, site, newAssembly);

            // The renewal rules read the site's flags through test.Site; a separate copy carries them so
            // the new test's own Site navigation stays unset.
            test.RenewalRequired = await _testService.IsRenewalRequiredAsync(new BackflowTest
            {
                WaterSupplierId = test.WaterSupplierId,
                DeviceType = test.DeviceType,
                HazardType = test.HazardType,
                PropertyType = test.PropertyType,
                Site = site
            }, cancellationToken);

            var assembly = LinkToInspection(BuildAssembly(test), inspection, newAssembly.VisuallyIdentified);
            assembly.AddedOnInspection = true;
            assembly.Test = test;

            assemblies.Add(assembly);
        }

        return assemblies;
    }

    private static BackflowTest BuildTest(CsiInspection inspection, Site site, CsiInspectionNewAssemblyDto newAssembly)
    {
        var inspectionDate = inspection.InspectionDate;

        return new BackflowTest
        {
            WaterSupplierId = inspection.WaterSupplierId,
            SiteId = inspection.SiteId,
            ProfessionalId = inspection.ProfessionalId,
            InspectorId = inspection.InspectorId,

            DeviceType = newAssembly.DeviceType,
            Manufacturer = newAssembly.Manufacturer,
            Model = newAssembly.Model,
            Size = newAssembly.Size,
            SerialNumber = newAssembly.SerialNumber?.Trim(),
            Manufacturer2 = newAssembly.Manufacturer2,
            Model2 = newAssembly.Model2,
            Size2 = newAssembly.Size2,
            SerialNumber2 = newAssembly.SerialNumber2?.Trim(),
            HazardType = newAssembly.HazardType,
            HazardTypeOtherDescription = newAssembly.HazardTypeOtherDescription?.Trim(),
            LocationDescription = newAssembly.LocationDescription?.Trim(),
            Comments = newAssembly.Comments?.Trim(),

            ReasonForTest = BackflowReasonForTest.AnnualTest,
            IsCurrent = true,
            NeedsValidation = true,

            InitialTestDate = inspectionDate,
            RepairTestDate = inspectionDate,
            FinalTestDate = inspectionDate,
            AirGapTestDate = inspectionDate,
            TestDate = inspectionDate,
            ExpirationDate = inspectionDate,

            AccountNumber = site.AccountNumber,
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
    }

    private static void ValidateNewAssembly(CsiInspectionNewAssemblyDto newAssembly)
    {
        if (!Enum.GetNames<BackflowDeviceType>().Contains(newAssembly.DeviceType))
        {
            throw new AppValidationException("Please select a valid Backflow Method.");
        }

        var mainIsIncomplete = string.IsNullOrWhiteSpace(newAssembly.Manufacturer)
            || string.IsNullOrWhiteSpace(newAssembly.Model)
            || string.IsNullOrWhiteSpace(newAssembly.Size)
            || string.IsNullOrWhiteSpace(newAssembly.SerialNumber);

        if (newAssembly.DeviceType != nameof(BackflowDeviceType.AG) && mainIsIncomplete)
        {
            throw new AppValidationException("Main Assembly Manufacturer, Model, Size and Serial Number are required for this backflow method.");
        }

        var bypassIsIncomplete = string.IsNullOrWhiteSpace(newAssembly.Manufacturer2)
            || string.IsNullOrWhiteSpace(newAssembly.Model2)
            || string.IsNullOrWhiteSpace(newAssembly.Size2)
            || string.IsNullOrWhiteSpace(newAssembly.SerialNumber2);

        if (BackflowDeviceTypes.HasBypassAssembly(newAssembly.DeviceType) && bypassIsIncomplete)
        {
            throw new AppValidationException("Bypass Assembly Manufacturer, Model, Size and Serial Number are required for this backflow method.");
        }

        if (newAssembly.HazardType == "Other" && string.IsNullOrWhiteSpace(newAssembly.HazardTypeOtherDescription))
        {
            throw new AppValidationException("Other Description is required when the Hazard Type is Other.");
        }
    }

    private static CsiInspectionVisuallyIdentifiedAssembly LinkToInspection(
        CsiInspectionVisuallyIdentifiedAssembly assembly,
        CsiInspection inspection,
        bool visuallyIdentified)
    {
        assembly.WaterSupplierId = inspection.WaterSupplierId;
        assembly.InspectionId = inspection.Id;
        assembly.VisuallyIdentified = visuallyIdentified;
        assembly.CreatedTime = DateTime.UtcNow;

        return assembly;
    }

    // V1 addVisuallyIdentifiedAssemblyFromTest: the row is a snapshot of the test.
    private static CsiInspectionVisuallyIdentifiedAssembly BuildAssembly(BackflowTest test)
    {
        var hasBypass = BackflowDeviceTypes.HasBypassAssembly(test.DeviceType);

        return new CsiInspectionVisuallyIdentifiedAssembly
        {
            WaterSupplierId = test.WaterSupplierId,
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
            TransactionId = test.TransactionId
        };
    }

    // V1 BackflowTest.DeviceDescription.
    private static string BuildDescription(string? manufacturer, string? model, string? size, string? deviceType)
    {
        return $"{manufacturer} {model} {size} - {deviceType}";
    }
}
