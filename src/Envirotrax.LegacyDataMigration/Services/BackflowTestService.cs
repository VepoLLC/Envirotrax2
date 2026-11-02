using System.Linq.Expressions;
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Backflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class BackflowTestService
{
    private const string ScriptsFolder = "Scripts/BackflowTests";

    // The legacy file server or Azure Storage being down fails every image the same way. Stopping early
    // keeps a dead host from costing thousands of timeouts; nothing is written, so a later run resumes.
    private const int MaxConsecutiveFailures = 10;

    // Tests have several times more images than any other entity, plus a guess to check for every test a
    // supplier saved, so they are downloaded and uploaded a batch at a time instead of one by one.
    private const int ImagesPerBatch = 8;

    // A run takes hours, so the log says how far it has got every this many images.
    private const int ProgressLogInterval = 1000;

    private readonly ILogger<BackflowTestService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly LegacyFileServerService _legacyFileServerService;
    private readonly BlobStorageService _blobStorageService;

    public BackflowTestService(
        ILogger<BackflowTestService> logger,
        AppDbContext appDbContext,
        LegacyFileServerService legacyFileServerService,
        BlobStorageService blobStorageService)
    {
        _logger = logger;
        _appDbContext = appDbContext;
        _legacyFileServerService = legacyFileServerService;
        _blobStorageService = blobStorageService;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of backflow tests ---------");

        await ExecuteSqlScriptsAsync();
        await MoveImagesToAzureAsync();

        _logger.LogInformation("--------- Finished migration of backflow tests ---------");
    }

    private async Task ExecuteSqlScriptsAsync()
    {
        _logger.LogInformation("Executing database scripts from {folderName}.", ScriptsFolder);

        var scripts = Directory.GetFiles(ScriptsFolder, "*.sql").OrderBy(file => file);

        foreach (var file in scripts)
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            var addedRows = await _appDbContext.Database.ExecuteSqlRawAsync(sql);
            _logger.LogInformation("Imported backflow tests. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }

    private async Task MoveImagesToAzureAsync()
    {
        _logger.LogInformation("Moving legacy backflow test images into Azure Storage.");

        try
        {
            await _blobStorageService.VerifyCanWriteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Cannot write to Azure Storage, so no backflow test image was moved and every one of them is still pending. Sign in with an account that has the Storage Blob Data Contributor role on the configured storage account, then run the migration again. Error message: {errorMessage}.",
                ex.Message);

            return;
        }

        await MovePendingImagesAsync();
    }

    // Copies every queued image that has not been copied yet into Azure Storage. Each image path stays NULL
    // until that image really is in Azure, so a test whose image did not make it simply shows none instead
    // of a link that goes nowhere.
    private async Task MovePendingImagesAsync()
    {
        // The image paths are the completion flags: each is written only after its upload returned, so
        // anything still NULL has not made it to Azure yet and anything set is never touched again.
        // Read untracked, because tracking every pending row makes each save rescan all of them.
        var pendingTests = await _appDbContext
            .BackflowTests
            .AsNoTracking()
            .Where(test => test.LegacyRecordId != null
                && ((test.LegacyAssemblyImagePath != null && test.AssemblyImagePath == null)
                    || (test.LegacySerialNumberImagePath != null && test.SerialNumberImagePath == null)
                    || (test.LegacyBypassAssemblyImagePath != null && test.BypassAssemblyImagePath == null)
                    || (test.LegacyBypassSerialNumberImagePath != null && test.BypassSerialNumberImagePath == null)
                    || (test.LegacyAirGapImagePath != null && test.AirGapImagePath == null)))
            .OrderBy(test => test.Id)
            .ToListAsync();

        var pendingImages = pendingTests
            .SelectMany(GetPendingImages)
            .ToList();

        _logger.LogInformation("Moving {Count} images of {TestCount} backflow tests.", pendingImages.Count, pendingTests.Count);

        var processedCount = 0;
        var nextProgressLogCount = ProgressLogInterval;
        var movedCount = 0;
        var notFoundCount = 0;
        var unavailableCount = 0;
        var failedCount = 0;
        var consecutiveFailureCount = 0;

        foreach (var batch in pendingImages.Chunk(ImagesPerBatch))
        {
            var results = await Task.WhenAll(batch.Select(MoveImageAsync));

            try
            {
                await SaveBatchAsync(results);
            }
            catch (Exception ex)
            {
                // Nothing of the batch was recorded, so all of it is still pending for the next run, which
                // uploads the same blobs again over the ones this run left behind.
                results = results
                    .Select(result => new ImageMoveResult(result.Image, FailureMessage: ex.Message))
                    .ToArray();
            }

            foreach (var result in results)
            {
                var test = result.Image.Test;

                if (result.UploadedPath != null)
                {
                    movedCount++;
                    consecutiveFailureCount = 0;
                }
                else if (result.UnavailableReason != null)
                {
                    // A guess for a test a supplier saved usually finds nothing, and saying so for every one
                    // of them would bury the warnings that matter.
                    if (test.LegacyImagesUnrecorded == true)
                    {
                        notFoundCount++;
                    }
                    else
                    {
                        _logger.LogWarning(
                            "The {kind} image of backflow test {testId} was not moved because {reason}. Legacy path: {legacyFilePath}.",
                            result.Image.Kind, test.Id, result.UnavailableReason, result.Image.LegacyFilePath);

                        unavailableCount++;
                    }

                    // The source answered, so it is alive. Only failures that look like an outage count
                    // towards stopping the run early.
                    consecutiveFailureCount = 0;
                }
                else
                {
                    _logger.LogWarning(
                        "Failed moving the {kind} image of backflow test {testId}. Error message: {errorMessage}.",
                        result.Image.Kind, test.Id, result.FailureMessage);

                    failedCount++;
                    consecutiveFailureCount++;
                }
            }

            processedCount += results.Length;

            if (processedCount >= nextProgressLogCount)
            {
                _logger.LogInformation("Processed {ProcessedCount} of {Count} backflow test images.", processedCount, pendingImages.Count);
                nextProgressLogCount += ProgressLogInterval;
            }

            if (consecutiveFailureCount >= MaxConsecutiveFailures)
            {
                _logger.LogError(
                    "Stopping after {failureCount} backflow test images failed in a row. The legacy file server, Azure Storage or the database is most likely unreachable. Everything left is still pending, so run the migration again once it is back.",
                    consecutiveFailureCount);

                break;
            }
        }

        _logger.LogInformation(
            "Moved {MovedCount} backflow test images, found nothing at {NotFoundCount} paths guessed for tests a supplier saved, could not get {UnavailableCount} recorded images out of V1, and failed {FailedCount} images.",
            movedCount, notFoundCount, unavailableCount, failedCount);
    }

    // The images of one test that are still waiting to be copied. Kind is the V2 folder each goes into,
    // the same names Envirotrax.App's BackflowTestService uses.
    private static IEnumerable<PendingImage> GetPendingImages(BackflowTest test)
    {
        if (test.LegacyAssemblyImagePath != null && test.AssemblyImagePath == null)
        {
            yield return new PendingImage(
                test,
                "assembly",
                test.LegacyAssemblyImagePath,
                pendingTest => pendingTest.AssemblyImagePath,
                pendingTest => pendingTest.LegacyAssemblyImagePath);
        }

        if (test.LegacySerialNumberImagePath != null && test.SerialNumberImagePath == null)
        {
            yield return new PendingImage(
                test,
                "serial-number",
                test.LegacySerialNumberImagePath,
                pendingTest => pendingTest.SerialNumberImagePath,
                pendingTest => pendingTest.LegacySerialNumberImagePath);
        }

        if (test.LegacyBypassAssemblyImagePath != null && test.BypassAssemblyImagePath == null)
        {
            yield return new PendingImage(
                test,
                "bypass-assembly",
                test.LegacyBypassAssemblyImagePath,
                pendingTest => pendingTest.BypassAssemblyImagePath,
                pendingTest => pendingTest.LegacyBypassAssemblyImagePath);
        }

        if (test.LegacyBypassSerialNumberImagePath != null && test.BypassSerialNumberImagePath == null)
        {
            yield return new PendingImage(
                test,
                "bypass-serial-number",
                test.LegacyBypassSerialNumberImagePath,
                pendingTest => pendingTest.BypassSerialNumberImagePath,
                pendingTest => pendingTest.LegacyBypassSerialNumberImagePath);
        }

        if (test.LegacyAirGapImagePath != null && test.AirGapImagePath == null)
        {
            yield return new PendingImage(
                test,
                "air-gap",
                test.LegacyAirGapImagePath,
                pendingTest => pendingTest.AirGapImagePath,
                pendingTest => pendingTest.LegacyAirGapImagePath);
        }
    }

    // Downloads one image and uploads it, and touches no database, so a whole batch can run at once.
    private async Task<ImageMoveResult> MoveImageAsync(PendingImage image)
    {
        try
        {
            var download = await _legacyFileServerService.DownloadAsync(image.LegacyFilePath);

            if (download.Content == null)
            {
                return new ImageMoveResult(image, UnavailableReason: download.UnavailableReason ?? "the legacy file server returned no file");
            }

            // The folders V2 uses when a supplier replaces a test image, with the V1 record id in place of a
            // new GUID. A migrated test keeps its V1 id, so the name stays the same even when V2 is rebuilt
            // from scratch, and a retry overwrites the same blob instead of orphaning one.
            var blobPath = $"backflow-tests/{image.Test.Id}/{image.Kind}/legacy-{image.Test.LegacyRecordId}{Path.GetExtension(image.LegacyFilePath)}";

            using (download.Content)
            {
                var uploadedPath = await _blobStorageService.UploadAsync(blobPath, download.Content);

                return new ImageMoveResult(image, UploadedPath: uploadedPath);
            }
        }
        catch (Exception ex)
        {
            return new ImageMoveResult(image, FailureMessage: ex.Message);
        }
    }

    // One write for the whole batch. A moved image gets its V2 path. A guessed image the file server does
    // not have loses its legacy path instead, so no later run asks for it again; a recorded one stays
    // queued and is tried again, the way the other image migrations treat a missing file.
    private async Task SaveBatchAsync(IEnumerable<ImageMoveResult> results)
    {
        var changedTests = new Dictionary<int, BackflowTest>();

        foreach (var result in results)
        {
            var test = result.Image.Test;

            Expression<Func<BackflowTest, string?>> changedColumn;
            string? newValue;

            if (result.UploadedPath != null)
            {
                changedColumn = result.Image.PathProperty;
                newValue = result.UploadedPath;
            }
            else if (result.UnavailableReason != null && test.LegacyImagesUnrecorded == true)
            {
                changedColumn = result.Image.LegacyPathProperty;
                newValue = null;
            }
            else
            {
                continue;
            }

            if (!changedTests.TryGetValue(test.Id, out var changedTest))
            {
                // Only the key is set, so the save updates just the columns marked below and reads nothing.
                changedTest = new BackflowTest { WaterSupplierId = test.WaterSupplierId, Id = test.Id };
                _appDbContext.BackflowTests.Attach(changedTest);
                changedTests.Add(test.Id, changedTest);
            }

            var column = _appDbContext.Entry(changedTest).Property(changedColumn);
            column.CurrentValue = newValue;
            column.IsModified = true;
        }

        if (changedTests.Count == 0)
        {
            return;
        }

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        finally
        {
            _appDbContext.ChangeTracker.Clear();
        }
    }

    // PathProperty is the column set once the image is in Azure; LegacyPathProperty the one cleared when a
    // guessed image turns out not to exist.
    private record PendingImage(
        BackflowTest Test,
        string Kind,
        string LegacyFilePath,
        Expression<Func<BackflowTest, string?>> PathProperty,
        Expression<Func<BackflowTest, string?>> LegacyPathProperty);

    // What happened to one image: UploadedPath when it is in Azure now, UnavailableReason when V1 has no
    // usable file for it, FailureMessage when it should simply be tried again by the next run.
    private record ImageMoveResult(
        PendingImage Image,
        string? UploadedPath = null,
        string? UnavailableReason = null,
        string? FailureMessage = null);
}
