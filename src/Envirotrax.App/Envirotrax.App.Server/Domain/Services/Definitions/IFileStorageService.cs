
using Azure.Storage.Blobs.Models;

namespace Envirotrax.App.Server.Domain.Services.Definitions;

public interface IFileStorageService
{
    Task<string> UploadAsync(string filePath, Stream fileStream);

    Task<Uri> GenerateSasUrlAsync(string blobName);

    // Use this function when need to generate SAS URLs in a foreach loop. First, generate delegation key, then pass it to this funciton in a foreach loop.
    Task<Uri> GenerateSasUrlAsync(UserDelegationKey delegationKey, string blobName);
}