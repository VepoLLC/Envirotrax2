namespace Envirotrax.Website.Templates.Emails;

public class RequestInformationVm
{
    public string ProgramType { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string ContactName { get; set; } = null!;
    public string InformationType { get; set; } = null!;
    public string? MailingAddress { get; set; }
    public string? MailingCity { get; set; }
    public string? MailingState { get; set; }
    public string? MailingZip { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? Comments { get; set; }
}
