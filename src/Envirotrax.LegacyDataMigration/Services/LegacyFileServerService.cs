
using System.Net;

namespace Envirotrax.LegacyDataMigration.Services;

// The outcome of asking the legacy file server for one attachment. Content is set only when a usable
// file came back; otherwise UnavailableReason says why there is nothing to upload. Problems that are
// worth retrying later - the server being unreachable, a 500, a timeout - throw instead, so the caller
// can tell a file that is gone for good from a run that should simply be repeated.
public class LegacyFileDownload
{
    public MemoryStream? Content { get; init; }

    public string? UnavailableReason { get; init; }
}

// Reads V1 file attachments off the old file server. Everything below the server address comes from
// SiteLogs.LegacyFilePath, so this class never needs to know how V1 laid its folders out.
public class LegacyFileServerService
{
    private static readonly string[] HtmlExtensions = [".htm", ".html"];

    private readonly HttpClient _httpClient;

    public LegacyFileServerService(string fileServerAddress)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(fileServerAddress.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromMinutes(2)
        };
    }

    public async Task<LegacyFileDownload> DownloadAsync(string filePath)
    {
        using var response = await _httpClient.GetAsync(filePath, HttpCompletionOption.ResponseHeadersRead);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new LegacyFileDownload { UnavailableReason = "the legacy file server answered 404 Not Found" };
        }

        response.EnsureSuccessStatusCode();

        // The legacy file server is IIS in front of a file share, so a custom error page can come back
        // with a 200. Uploading that would leave a working looking link to a page of error markup,
        // which is worse than leaving the file name as plain text.
        var contentType = response.Content.Headers.ContentType?.MediaType;

        if (contentType == "text/html" && !HtmlExtensions.Contains(Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase))
        {
            return new LegacyFileDownload { UnavailableReason = "the legacy file server answered with an HTML page instead of a file" };
        }

        // Buffered rather than streamed so the blob SDK gets a seekable stream of known length, and so
        // the HTTP response is disposed before the upload starts. Only the headers have been read up
        // to here, so the attachment is copied once instead of being held twice while it downloads.
        var content = new MemoryStream();
        await response.Content.CopyToAsync(content);

        if (content.Length == 0)
        {
            await content.DisposeAsync();

            return new LegacyFileDownload { UnavailableReason = "the legacy file server answered with an empty file" };
        }

        content.Position = 0;

        return new LegacyFileDownload { Content = content };
    }
}
