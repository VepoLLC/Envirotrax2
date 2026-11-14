namespace Envirotrax.App.Server.Data.Models.Sites;

// FilePath is the Azure blob name and must never be returned by the API.
public class SiteLetterLogFile
{
    public DateTime ProcessDate { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int LetterCount { get; set; }
}
