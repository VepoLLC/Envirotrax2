using System.IO.Compression;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Letters;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Letters;

public class LetterProcessService : ILetterProcessService
{
    private const string ZipFileNameFormat = "Vepo PDF's Generated for {0}.zip";
    private const int ZipFileBufferSize = 81920;

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

        await _fileStorageService.DeleteAsync(GetZipFilePath(processDate));

        return true;
    }

    public async Task<bool> CreateZipFileAsync(string processId)
    {
        if (!LetterProcessId.TryGetProcessDate(processId, out var processDate))
        {
            return false;
        }

        await TryCreateZipFileAsync(processDate);

        return true;
    }

    public async Task<Uri?> GenerateZipFileUrlAsync(string processId, CancellationToken cancellationToken)
    {
        if (!LetterProcessId.TryGetProcessDate(processId, out var processDate))
        {
            return null;
        }

        var zipFilePath = GetZipFilePath(processDate);
        var zipFileExists = await _fileStorageService.ExistsAsync(zipFilePath, cancellationToken);

        if (!zipFileExists)
        {
            var isCreated = await TryCreateZipFileAsync(processDate);

            if (!isCreated)
            {
                return null;
            }
        }

        return await _fileStorageService.GenerateSasUrlAsync(zipFilePath);
    }

    private async Task<bool> TryCreateZipFileAsync(DateTime processDate)
    {
        var pdfFiles = await _siteLetterLogRepository.GetPdfFilesAsync(processDate, CancellationToken.None);

        if (pdfFiles.Count == 0)
        {
            return false;
        }

        await using var zipStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, ZipFileBufferSize, FileOptions.DeleteOnClose);

        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var pdfFile in pdfFiles)
            {
                var entry = archive.CreateEntry(pdfFile.FileName, CompressionLevel.Fastest);

                await using var entryStream = entry.Open();

                await _fileStorageService.DownloadAsync(pdfFile.FilePath, entryStream);
            }
        }

        zipStream.Position = 0;

        await _fileStorageService.UploadAsync(GetZipFilePath(processDate), zipStream);

        return true;
    }

    private static string GetZipFilePath(DateTime processDate)
    {
        var processId = LetterProcessId.FromProcessDate(processDate);
        var zipFileName = string.Format(ZipFileNameFormat, processId);

        return $"{LetterProcessId.GetFolderPath(processDate)}/{zipFileName}";
    }
}
