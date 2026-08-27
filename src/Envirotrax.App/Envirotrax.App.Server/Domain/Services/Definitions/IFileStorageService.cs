
using Azure.Storage.Blobs.Models;

namespace Envirotrax.App.Server.Domain.Services.Definitions;

public interface IFileStorageService
{
    Task<string> UploadAsync(string filePath, Stream fileStream);

    Task<UserDelegationKey> GetUserDelegationKeyAsync();

    Task<Uri> GenerateSasUrlAsync(string blobName);

    Task<Uri> GenerateSasUrlAsync(UserDelegationKey delegationKey, string blobName);

    Task<bool> DeleteAsync(string blobName);
}