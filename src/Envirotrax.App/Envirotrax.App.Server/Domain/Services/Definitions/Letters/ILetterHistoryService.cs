using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Letters;

public interface ILetterHistoryService
{
    Task<List<LetterHistoryDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<Uri?> GenerateFileUrlAsync(string processId, string fileName, CancellationToken cancellationToken);
}
