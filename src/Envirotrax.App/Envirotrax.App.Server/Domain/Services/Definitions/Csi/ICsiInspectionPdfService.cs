using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectionPdfService
{
    Task<byte[]> GenerateAsync(CsiInspectionDto inspection);
}
