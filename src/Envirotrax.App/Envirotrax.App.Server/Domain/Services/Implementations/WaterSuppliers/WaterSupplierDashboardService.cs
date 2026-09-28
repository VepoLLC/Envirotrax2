using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierDashboardService : IWaterSupplierDashboardService
{
    private readonly IWaterSupplierDashboardRepository _repository;
    private readonly IBackflowComplianceReportService _complianceReportService;
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IBackflowGaugeService _gaugeService;

    public WaterSupplierDashboardService(
        IWaterSupplierDashboardRepository repository,
        IBackflowComplianceReportService complianceReportService,
        IProfessionalUserLicenseService licenseService,
        IBackflowGaugeService gaugeService)
    {
        _repository = repository;
        _complianceReportService = complianceReportService;
        _licenseService = licenseService;
        _gaugeService = gaugeService;
    }

    public async Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var stats = await _repository.GetStatsAsync(cancellationToken);
        var licenseCounts = await _licenseService.GetCountsByWaterSupplierAsync(cancellationToken);

        stats.UnverifiedLicenseCount = licenseCounts.UnverifiedCount;
        stats.ExpiredLicenseCount = licenseCounts.ExpiredCount;
        stats.ExpiringLicenseCount = licenseCounts.ExpiringCount;

        stats.TestGaugeCount = await _gaugeService.GetUnverifiedCountByWaterSupplierAsync(cancellationToken);
        stats.TransporterRegistrationCount = await _licenseService.GetUnverifiedRegistrationCountByWaterSupplierAsync(cancellationToken);

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
