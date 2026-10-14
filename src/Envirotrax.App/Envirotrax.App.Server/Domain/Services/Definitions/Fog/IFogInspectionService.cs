using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogInspectionService : IService<FogInspection, FogInspectionDto>
{
    Task<FogInspectionDto> SubmitAsync(
        FogInspectionDto request,
        Stream? exteriorStream, string? exteriorFileName,
        Stream? interiorStream, string? interiorFileName,
        Stream? signatureStream, string? signatureFileName,
        CancellationToken cancellationToken);

    Task<FogInspectionDto?> UpdateForProfessionalAsync(
        int id,
        FogInspectionDto request,
        Stream? exteriorStream, string? exteriorFileName,
        Stream? interiorStream, string? interiorFileName,
        Stream? signatureStream, string? signatureFileName,
        CancellationToken cancellationToken);

    Task<FogInspectionDto?> UpdateForAdminAsync(int id, FogInspectionAdminUpdateRequest request);

    Task<FogInspectionDto?> UpdateImageForAdminAsync(int id, string imageType, Stream fileStream, string fileName);

    Task<IPagedData<FogInspectionDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken);

    Task<FogInspectionDto?> GetForProfessionalAsync(int id, CancellationToken cancellationToken);

    Task<byte[]> GeneratePdfAsync(FogInspectionDto inspection);

    Task<byte[]> GeneratePdfAsync(IEnumerable<FogInspectionDto> inspections);

    Task<byte[]> GeneratePdfForProfessionalAsync(FogInspectionDto inspection);
}
