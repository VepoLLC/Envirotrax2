using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Letters;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Letters;

public class LetterProcessService : ILetterProcessService
{
    private readonly ISiteLetterLogRepository _siteLetterLogRepository;
    private readonly IFileStorageService _fileStorageService;

    public LetterProcessService(ISiteLetterLogRepository siteLetterLogRepository, IFileStorageService fileStorageService)
    {
        _siteLetterLogRepository = siteLetterLogRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<LetterProcessDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var processDates = await _siteLetterLogRepository.GetProcessDatesAsync(cancellationToken);
        var processes = new List<LetterProcessDto>();

        foreach (var processDate in processDates)
        {
            processes.Add(new LetterProcessDto
            {
                Id = LetterProcessId.FromProcessDate(processDate),
                ProcessDate = processDate
            });
        }

        return processes;
    }

    public async Task<List<LetterFileDto>?> GetFilesAsync(string processId, CancellationToken cancellationToken)
    {
        if (!LetterProcessId.TryGetProcessDate(processId, out var processDate))
        {
            return null;
        }

        return await _siteLetterLogRepository.GetFilesAsync(processDate, cancellationToken);
    }

    public async Task<List<LetterEmailDto>?> GetEmailsAsync(string processId, CancellationToken cancellationToken)
    {
        if (!LetterProcessId.TryGetProcessDate(processId, out var processDate))
        {
            return null;
        }

        return await _siteLetterLogRepository.GetEmailsAsync(processDate, cancellationToken);
    }

    public async Task<Uri?> GenerateFileUrlAsync(string processId, string fileName, CancellationToken cancellationToken)
    {
        if (!LetterProcessId.TryGetProcessDate(processId, out var processDate))
        {
            return null;
        }

        var filePath = await _siteLetterLogRepository.GetFilePathAsync(processDate, fileName, cancellationToken);

        if (filePath == null)
        {
            return null;
        }

        return await _fileStorageService.GenerateSasUrlAsync(filePath);
    }

    public async Task<bool> DeleteAsync(string processId)
    {
        if (!LetterProcessId.TryGetProcessDate(processId, out var processDate))
        {
            return false;
        }

        var filePaths = await _siteLetterLogRepository.GetFilePathsAsync(processDate, CancellationToken.None);

        if (filePaths.Count == 0)
        {
            return false;
        }

        await _siteLetterLogRepository.DeleteByProcessDateAsync(processDate);

        foreach (var filePath in filePaths)
        {
            await _fileStorageService.DeleteAsync(filePath);
        }

        return true;
    }
}
