using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectionService : IService<CsiInspection, CsiInspectionDto>
{
    Task<CsiInspectionDto> SubmitAsync(CsiInspectionDto request, CancellationToken cancellationToken);
    Task<IPagedData<CsiInspectionDto>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken);
    Task<IPagedData<CsiInspectionDto>> SearchForAdminAsync(PageInfo pageInfo, Query query, CsiPaymentStatus? paymentStatus, CancellationToken cancellationToken);

    Task<IPagedData<CsiInspectionDto>> SearchForSubAccountAsync(PageInfo pageInfo, Query query, int subAccountWaterSupplierId, CancellationToken cancellationToken);
    Task<CsiInspectionDto?> UpdateApprovalAsync(int id, CsiInspectionApprovalRequest request, CancellationToken cancellationToken);
    Task<CsiInspectionDto?> UpdateForAdminAsync(int id, CsiInspectionAdminUpdateRequest request);
    Task<byte[]> GeneratePdfAsync(CsiInspectionDto inspection);
    Task<byte[]> GeneratePdfAsync(IEnumerable<CsiInspectionDto> inspections);
    Task<byte[]> GeneratePdfForProfessionalAsync(CsiInspectionDto inspection);
}
