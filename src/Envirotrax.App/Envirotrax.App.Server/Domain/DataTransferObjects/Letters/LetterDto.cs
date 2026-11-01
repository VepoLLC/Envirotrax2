namespace Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

public class LetterDto
{
    public int SiteId { get; set; }

    public List<string> MailingLines { get; set; } = [];

    public string AccountNumber { get; set; } = null!;
    public List<string> PropertyLines { get; set; } = [];

    public List<LetterAssemblyDto> Assemblies { get; set; } = [];

    public string OptInUrl { get; set; } = null!;
    public string OptInCode { get; set; } = null!;
    public string OptInExpirationDate { get; set; } = null!;
}

public class LetterAssemblyDto
{
    public string Description { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? LocationDescription { get; set; }
    public bool IsFireSystem { get; set; }
    public string? TesterCompanyName { get; set; }
    public string? TesterPhoneNumber { get; set; }
    public string? TestDate { get; set; }
    public string? ExpirationDate { get; set; }
}
