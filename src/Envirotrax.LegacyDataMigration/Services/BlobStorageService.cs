
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Envirotrax.LegacyDataMigration.Services;

// Writes files into the same Azure Storage container Envirotrax.App reads them back from. This is a
// console app and must not reference the web application, so only the upload half of App.Server's
// FileStorageService is duplicated here - the migration never generates a SAS URL and never deletes.
public class BlobStorageService
{
    private const string DefaultContentType = "application/octet-stream";

    // App.Server derives the content type with FileExtensionContentTypeProvider, which lives in
    // Microsoft.AspNetCore.StaticFiles and would drag the whole ASP.NET Core shared framework into a
    // console app. These are the same values for the file types V2 accepts; anything else is uploaded
    // as a plain download rather than rejected, because the legacy server is going away.
    private static readonly Dictionary<string, string> ContentTypesByExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".bmp"] = "image/bmp",
        [".gif"] = "image/gif",
        [".tif"] = "image/tiff",
        [".tiff"] = "image/tiff",
        [".pdf"] = "application/pdf",
        [".txt"] = "text/plain",
        [".doc"] = "application/msword",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".xls"] = "application/vnd.ms-excel",
        [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };

    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(string accountName, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(
            new Uri($"https://{accountName}.blob.core.windows.net"),
            new DefaultAzureCredential());

        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    // Proves the migration can really write before it starts downloading thousands of files. Only
    // checking that the container exists would pass for a read-only sign-in and then fail on every
    // single upload. Throws with the underlying reason, and warms up the DefaultAzureCredential chain.
    public async Task VerifyCanWriteAsync()
    {
        var blobClient = _containerClient.GetBlobClient($"migration-write-probe/{Guid.NewGuid()}.txt");

        using var probeContent = new MemoryStream([0]);

        await blobClient.UploadAsync(probeContent, overwrite: true);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<string> UploadAsync(string filePath, Stream fileStream)
    {
        var blobClient = _containerClient.GetBlobClient(filePath);

        // No access conditions, so a retry after an interrupted run overwrites the same blob in place
        // instead of leaving an orphan behind.
        await blobClient.UploadAsync(fileStream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(filePath)
            }
        });

        return blobClient.Name;
    }

    private static string GetContentType(string filePath)
    {
        if (!ContentTypesByExtension.TryGetValue(Path.GetExtension(filePath), out var contentType))
        {
            return DefaultContentType;
        }

        return contentType;
    }
}
