using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowTestService : IService<BackflowTest, BackflowTestDto>
{
    Task<BackflowTestDto> SubmitWithImagesAsync(
        BackflowTestDto dto,
        Stream? assemblyStream, string? assemblyFileName,
        Stream? serialStream, string? serialFileName,
        Stream? bypassAssemblyStream, string? bypassAssemblyFileName,
        Stream? bypassSerialStream, string? bypassSerialFileName,
        Stream? airGapStream, string? airGapFileName,
        CancellationToken cancellationToken = default);

    Task<BackflowTestDto?> UpdateImageAsync(int id, string imageType, Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    Task<BackflowTestExpiryCountsDto> GetExpiryCountsAsync(CancellationToken cancellationToken = default);

    Task<IPagedData<BackflowComplianceDto>> GetComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<byte[]> GeneratePdfAsync(BackflowTestDto test);
    Task<byte[]> GeneratePdfAsync(IEnumerable<BackflowTestDto> tests);

    Task ProcessSiteRenewalAsync(int siteId, CancellationToken cancellationToken);
    Task ProcessTestRenewalAsync(int testId, CancellationToken cancellationToken);
    Task<IEnumerable<BackflowTestDto>> GetAllPendingTestsForRenewalAsync(int batchSize, CancellationToken cancellationToken);

    Task<BackflowTestDto?> UpdateRenewalRequiredAsync(int id, bool renewalRequired, CancellationToken cancellationToken = default);
    Task<BackflowTestDto?> UpdateScheduleMonthAsync(int id, int month, CancellationToken cancellationToken = default);
    Task<BackflowTestDto?> UpdateIsCurrentAsync(int id, bool isCurrent, CancellationToken cancellationToken = default);
    Task<BackflowTestDto?> UpdateOutOfServiceAsync(int id, bool outOfService, CancellationToken cancellationToken = default);
    Task<BackflowTestDto?> UpdateDisapprovalAsync(int id, bool disapproved, CancellationToken cancellationToken = default);
    Task<BackflowTestDto?> UpdateForceRenewalAsync(int id, BackflowTestForceRenewalRequest request, CancellationToken cancellationToken = default);
    Task<BackflowTestDto?> UpdateRejectionAsync(int id, BackflowTestRejectionRequest request, CancellationToken cancellationToken = default);
}
