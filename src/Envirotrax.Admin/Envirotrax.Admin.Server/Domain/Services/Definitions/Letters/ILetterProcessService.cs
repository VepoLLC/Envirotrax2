using Envirotrax.Admin.Server.Domain.DataTransferObjects.Letters;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Letters;

public interface ILetterProcessService
{
    Task<List<LetterProcessDto>?> GetAllAsync(CancellationToken cancellationToken);

    Task<List<LetterFileDto>?> GetFilesAsync(string processId, CancellationToken cancellationToken);

    Task<List<LetterEmailDto>?> GetEmailsAsync(string processId, CancellationToken cancellationToken);

    Task<string?> GetFileUrlAsync(string processId, string fileName, CancellationToken cancellationToken);

    Task DeleteAsync(string processId);
}
