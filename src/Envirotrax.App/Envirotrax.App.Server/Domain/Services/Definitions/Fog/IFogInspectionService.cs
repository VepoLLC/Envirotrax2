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

    Task<IPagedData<FogInspectionDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken);

    Task<byte[]> GeneratePdfAsync(FogInspectionDto inspection);

    Task<byte[]> GeneratePdfAsync(IEnumerable<FogInspectionDto> inspections);
}
