
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Azure.Identity;
using Envirotrax.App.Server.Domain.Configuration;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Envirotrax.App.Server.Domain.Services.Definitions;

namespace Envirotrax.App.Server.Domain.Services.Implementations;

public class FileStorageService : IFileStorageService
{

    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _defaultContainerClient;

    private static readonly char[] _invalidFIleNameChars = Path.GetInvalidFileNameChars();
    private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    public FileStorageService(IOptions<FileStorageOptions> storageOptions)
    {
        _blobServiceClient = new(
            new Uri($"https://{storageOptions.Value.AccountName}.blob.core.windows.net"),
            new DefaultAzureCredential());

        _defaultContainerClient = _blobServiceClient.GetBlobContainerClient(storageOptions.Value.DefaultContainerName);
    }

    private static string SanitizeFilePath(string filePath)
    {
        return new string([.. filePath.Where(c => c == '/' || !_invalidFIleNameChars.Contains(c))]);
    }

    private static string GetContentType(string filePath)
    {
        if (!_contentTypeProvider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }

    public async Task<string> UploadAsync(string filePath, Stream fileStream)
    {
        var cleanedPath = SanitizeFilePath(filePath);
        var blobClient = _defaultContainerClient.GetBlobClient(cleanedPath);

        await blobClient.UploadAsync(fileStream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(filePath)
            }
        });

        return blobClient.Name;
    }

    private async Task<UserDelegationKey> GetUserDelegationKeyAsync(int minutesValidFor)
    {
        var expiry = DateTimeOffset.UtcNow.AddMinutes(minutesValidFor);
        return (await _blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, expiry)).Value;
    }

    public Task<UserDelegationKey> GetUserDelegationKeyAsync()
    {
        return GetUserDelegationKeyAsync(2);
    }

    // Use this function when need to generate SAS URLs in a foreach loop. First, generate delegation key, then pass it to this funciton in a foreach loop.
    public async Task<Uri> GenerateSasUrlAsync(UserDelegationKey delegationKey, string blobName)
    {
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _defaultContainerClient.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = delegationKey.SignedExpiresOn
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasParams = sasBuilder.ToSasQueryParameters(delegationKey, _blobServiceClient.AccountName);

        var blobUri = _defaultContainerClient.GetBlobClient(blobName).Uri;
        var uriBuilder = new BlobUriBuilder(blobUri) { Sas = sasParams };

        return uriBuilder.ToUri();
    }

    public async Task<Uri> GenerateSasUrlAsync(string blobName)
    {
        var delegationKey = await GetUserDelegationKeyAsync();
        return await GenerateSasUrlAsync(delegationKey, blobName);
    }
}