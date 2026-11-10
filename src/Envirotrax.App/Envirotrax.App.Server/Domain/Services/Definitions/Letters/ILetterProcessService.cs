using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Letters;

public interface ILetterProcessService
{
    Task<List<LetterProcessDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<List<LetterFileDto>?> GetFilesAsync(string processId, CancellationToken cancellationToken);

    Task<Uri?> GenerateFileUrlAsync(string processId, string fileName, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string processId);
}
