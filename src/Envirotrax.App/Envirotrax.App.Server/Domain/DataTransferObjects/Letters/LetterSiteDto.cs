using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

public class LetterSiteDto
{
    public int Id { get; set; }

    public string AccountNumber { get; set; } = null!;
    public string? WaterSupplierAccountNumber { get; set; }

    public PropertyType PropertyType { get; set; }
    public string? BusinessName { get; set; }
    public string? StreetNumber { get; set; }
    public string? StreetName { get; set; }
    public string? PropertyNumber { get; set; }
    public string? City { get; set; }
    public string? StateCode { get; set; }
    public string? ZipCode { get; set; }

    public string? MailingCompanyName { get; set; }
    public string? MailingContactName { get; set; }
    public string? MailingStreetNumber { get; set; }
    public string? MailingStreetName { get; set; }
    public string? MailingNumber { get; set; }
    public string? MailingCity { get; set; }
    public string? MailingStateCode { get; set; }
    public string? MailingZipCode { get; set; }
    public string? MailingEmailAddress { get; set; }

    public RenewalOptInType RenewalOptInType { get; set; }

    public List<LetterBackflowTestDto> BackflowTests { get; set; } = [];
}
