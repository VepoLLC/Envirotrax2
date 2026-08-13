
using Azure.Identity;
using Azure.Storage.Blobs;
using Umbraco.Cms.Core.Composing;
using Umbraco.StorageProviders.AzureBlob.IO;

namespace Envirotrax.Website.Composers;

public class AzureBlobFileSystemComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        var storageAccount = builder.Config["Storage:AccountName"] ?? throw new InvalidOperationException("Storage account name is required.");
        var containerName = builder.Config["Storage:ContainerName"] ?? throw new InvalidOperationException("Storage container name is required.");

        builder.AddAzureBlobMediaFileSystem(options =>
        {
            options.ConnectionString = $"https://{storageAccount}.blob.core.windows.net";
            options.ContainerName = containerName;
            options.TryCreateBlobContainerClientUsingUri(uri => new BlobContainerClient(uri, new DefaultAzureCredential(), options.ConfigureRetry(new BlobClientOptions())));
        });
    }
}