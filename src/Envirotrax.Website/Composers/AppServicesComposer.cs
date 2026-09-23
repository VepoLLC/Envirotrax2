
using System.Reflection;
using Azure.Identity;
using Azure.Storage.Blobs;
using Envirotrax.Common.Configuration;
using Envirotrax.Website.Configuration;
using Umbraco.Cms.Core.Composing;
using Umbraco.StorageProviders.AzureBlob.IO;

namespace Envirotrax.Website.Composers;

public class AppServicesComposer : IComposer
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

        builder.Services.Configure<LegacyUrlRedirectOptions>(builder.Config.GetSection("LegacyUrlRedirects"));

        builder.Services.AddEmailService(builder.Config.GetSection("Email"), options =>
        {
            options.Assembly = Assembly.GetExecutingAssembly();
            options.Namespace = "Envirotrax.Website";
        });

        builder.Services.AddRecaptchaService(builder.Config.GetSection("Recaptcha"));
    }
}
