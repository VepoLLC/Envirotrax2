namespace Envirotrax.LegacyDataMigration.Services;

public class LegacyLetterFolderService
{
    public string LettersFolder { get; }

    public LegacyLetterFolderService(string lettersFolder)
    {
        LettersFolder = lettersFolder;
    }

    public async Task<LegacyFileDownload> ReadAsync(string filePath)
    {
        var fullPath = Path.Combine(LettersFolder, filePath);

        if (!File.Exists(fullPath))
        {
            return new LegacyFileDownload { UnavailableReason = "the file is not in the legacy letters folder" };
        }

        var bytes = await File.ReadAllBytesAsync(fullPath);

        if (bytes.Length == 0)
        {
            return new LegacyFileDownload { UnavailableReason = "the file in the legacy letters folder is empty" };
        }

        return new LegacyFileDownload { Content = new MemoryStream(bytes) };
    }
}
