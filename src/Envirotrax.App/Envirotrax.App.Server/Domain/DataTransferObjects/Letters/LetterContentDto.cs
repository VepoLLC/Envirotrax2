namespace Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

public class LetterContentDto
{
    public string FontFamily { get; set; } = null!;
    public int FontSize { get; set; }

    public string? Title { get; set; }
    public string HeaderBackgroundColor { get; set; } = null!;
    public string HeaderForegroundColor { get; set; } = null!;
    public string HeaderBorderColor { get; set; } = null!;

    public List<LetterMessageLineDto> MessageLines { get; set; } = [];

    public string LetterDate { get; set; } = null!;

    public string? ReturnCompanyName { get; set; }
    public string? ReturnAddress { get; set; }
    public string ReturnCityStateZip { get; set; } = null!;

    public bool HasMainContact { get; set; }
    public string? ContactCompanyName { get; set; }
    public string? ContactName { get; set; }
    public string? ContactAddress { get; set; }
    public string? ContactCityStateZip { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public string? ContactFaxNumber { get; set; }
    public string? ContactEmailAddress { get; set; }

    public string? SignatureName { get; set; }

    public bool ShowAssemblies { get; set; }
}
