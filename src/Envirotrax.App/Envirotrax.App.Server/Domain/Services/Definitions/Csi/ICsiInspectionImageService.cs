using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectionImageService
{
    Task<List<CsiInspectionImageDto>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<CsiInspectionImageDto> AddImageAsync(int inspectionId, string? description, Stream imageStream, string fileName, CancellationToken cancellationToken);
    Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken);
}
