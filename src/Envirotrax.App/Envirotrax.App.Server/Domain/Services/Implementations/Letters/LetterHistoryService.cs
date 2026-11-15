using System.Collections.Concurrent;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Letters;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Letters;

public class LetterHistoryService : ILetterHistoryService
{
    private const string LettersFolder = "letters";
    private const string PdfExtension = ".pdf";
    private const int FilePathSegmentCount = 4;
    private const int MaxConcurrentFolderListings = 8;

    private readonly ISiteLetterLogRepository _siteLetterLogRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITenantProvidersService _tenantProvider;
    private readonly ILogger<LetterHistoryService> _logger;

    public LetterHistoryService(
        ISiteLetterLogRepository siteLetterLogRepository,
        IFileStorageService fileStorageService,
        ITenantProvidersService tenantProvider,
        ILogger<LetterHistoryService> logger)
    {
        _siteLetterLogRepository = siteLetterLogRepository;
        _fileStorageService = fileStorageService;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public async Task<List<LetterHistoryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var waterSupplierId = _tenantProvider.WaterSupplierId;

        if (waterSupplierId <= 0)
        {
            return [];
        }

        var files = await _siteLetterLogRepository.GetGeneratedPdfFilesAsync(waterSupplierId, cancellationToken);
        var generatedFiles = GetGeneratedPdfFiles(waterSupplierId, files);

        var fileSizes = await GetFileSizesAsync(generatedFiles, cancellationToken);

        var processes = generatedFiles
            .GroupBy(file => file.ProcessDate)
            .OrderByDescending(group => group.Key);

        var history = new List<LetterHistoryDto>();

        foreach (var process in processes)
        {
            history.Add(CreateLetterHistory(process.Key, process, fileSizes));
        }

        return history;
    }

    public async Task<Uri?> GenerateFileUrlAsync(string processId, string fileName, CancellationToken cancellationToken)
    {
        var waterSupplierId = _tenantProvider.WaterSupplierId;

        if (waterSupplierId <= 0 || !LetterProcessId.TryGetProcessDate(processId, out var processDate) || !IsGeneratedPdfFileName(waterSupplierId, fileName))
        {
            return null;
        }

        var filePath = await _siteLetterLogRepository.GetGeneratedPdfFilePathAsync(waterSupplierId, processDate, fileName, cancellationToken);

        if (filePath == null || !IsGeneratedPdfFilePath(waterSupplierId, processDate, fileName, filePath))
        {
            return null;
        }

        return await _fileStorageService.GenerateSasUrlAsync(filePath);
    }

    private List<SiteLetterLogFile> GetGeneratedPdfFiles(int waterSupplierId, List<SiteLetterLogFile> files)
    {
        var generatedFiles = new List<SiteLetterLogFile>();

        foreach (var file in files)
        {
            if (IsGeneratedPdfFileName(waterSupplierId, file.FileName) && IsGeneratedPdfFilePath(waterSupplierId, file.ProcessDate, file.FileName, file.FilePath))
            {
                generatedFiles.Add(file);
            }
            else
            {
                _logger.LogWarning(
                    "Letter file {FileName} from {ProcessDate} is left out of the letter history of water supplier {WaterSupplierId} because its path {FilePath} does not match the generated letter layout.",
                    file.FileName, file.ProcessDate, waterSupplierId, file.FilePath);
            }
        }

        return generatedFiles;
    }

    private static string GetOwnerPrefix(int waterSupplierId)
    {
        return $"{waterSupplierId} - ";
    }

    private static bool IsGeneratedPdfFileName(int waterSupplierId, string fileName)
    {
        return fileName.StartsWith(GetOwnerPrefix(waterSupplierId), StringComparison.Ordinal)
            && fileName.EndsWith(PdfExtension, StringComparison.OrdinalIgnoreCase);
    }

    // Generated and migrated letters share one layout: letters/{process ID}/{supplier ID} - {supplier name}/{file name}.
    private static bool IsGeneratedPdfFilePath(int waterSupplierId, DateTime processDate, string fileName, string filePath)
    {
        var segments = filePath.Split('/');

        return segments.Length == FilePathSegmentCount
            && segments[0] == LettersFolder
            && segments[1] == LetterProcessId.FromProcessDate(processDate)
            && segments[2].StartsWith(GetOwnerPrefix(waterSupplierId), StringComparison.Ordinal)
            && segments[3] == fileName;
    }

    private async Task<IReadOnlyDictionary<string, long>> GetFileSizesAsync(List<SiteLetterLogFile> files, CancellationToken cancellationToken)
    {
        var filePaths = files.Select(file => file.FilePath).ToHashSet();
        var folderPaths = filePaths.Select(GetFolderPath).Distinct().ToList();

        var fileSizes = new ConcurrentDictionary<string, long>();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = MaxConcurrentFolderListings,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(folderPaths, parallelOptions, async (folderPath, folderCancellationToken) =>
        {
            try
            {
                var folderFileSizes = await _fileStorageService.GetFileSizesAsync(folderPath, folderCancellationToken);

                foreach (var (filePath, fileSize) in folderFileSizes)
                {
                    if (filePaths.Contains(filePath))
                    {
                        fileSizes[filePath] = fileSize;
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Could not list the letter files in {FolderPath}, so their sizes are not shown in the letter history.", folderPath);
            }
        });

        return fileSizes;
    }

    private static string GetFolderPath(string filePath)
    {
        return filePath[..filePath.LastIndexOf('/')];
    }

    private static LetterHistoryDto CreateLetterHistory(DateTime processDate, IEnumerable<SiteLetterLogFile> files, IReadOnlyDictionary<string, long> fileSizes)
    {
        var history = new LetterHistoryDto
        {
            Id = LetterProcessId.FromProcessDate(processDate),
            ProcessDate = processDate
        };

        foreach (var file in files.OrderBy(file => file.FileName, StringComparer.OrdinalIgnoreCase))
        {
            history.Files.Add(new LetterFileDto
            {
                FileName = file.FileName,
                LetterCount = file.LetterCount,
                FileSize = fileSizes.TryGetValue(file.FilePath, out var fileSize) ? fileSize : null
            });
        }

        return history;
    }
}
