using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierDashboardService : IWaterSupplierDashboardService
{
    private readonly IWaterSupplierDashboardRepository _repository;
    private readonly IBackflowComplianceReportService _complianceReportService;
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IProfessionalInsuranceService _insuranceService;

    public WaterSupplierDashboardService(
        IWaterSupplierDashboardRepository repository,
        IBackflowComplianceReportService complianceReportService,
        IProfessionalUserLicenseService licenseService,
        IProfessionalInsuranceService insuranceService)
    {
        _repository = repository;
        _complianceReportService = complianceReportService;
        _licenseService = licenseService;
        _insuranceService = insuranceService;
    }

    public async Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var stats = await _repository.GetStatsAsync(cancellationToken);

        // The license badges link into the License Management page, so they have to be counted the way
        // that page counts its tabs: expired last month, expiring this month, and only professionals
        // linked to this water supplier.
        var licenseCounts = await _licenseService.GetCountsByWaterSupplierAsync(cancellationToken);

        stats.UnverifiedLicenseCount = licenseCounts.UnverifiedCount;
        stats.ExpiredLicenseCount = licenseCounts.ExpiredCount;
        stats.ExpiringLicenseCount = licenseCounts.ExpiringCount;

        // Same reason for insurance: the badge opens the Insurance Management page on its Unverified tab,
        // so it has to be that tab's number and not a count across every water supplier in the database.
        var insuranceCounts = await _insuranceService.GetCountsByWaterSupplierAsync(cancellationToken);

        stats.InsurancePolicyCount = insuranceCounts.UnverifiedCount;

        return stats;
    }

    public Task<CsiSubmissionStatsDto> GetCsiSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetCsiSubmissionStatsAsync(cancellationToken);

        return stats;
    }

    public Task<BackflowSubmissionStatsDto> GetBackflowSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetBackflowSubmissionStatsAsync(cancellationToken);

        return stats;
    }

    public Task<BackflowComplianceSnapshotDto?> GetBackflowComplianceAsync(CancellationToken cancellationToken)
    {
        var compliance = _complianceReportService.GetLatestComplianceAsync(cancellationToken);

        return compliance;
    }

    public Task<FogInspectionSubmissionStatsDto> GetFogInspectionSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetFogInspectionSubmissionStatsAsync(cancellationToken);

        return stats;
    }

    public Task<FogTripTicketSubmissionStatsDto> GetFogTripTicketSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetFogTripTicketSubmissionStatsAsync(cancellationToken);

        return stats;
    }
}
