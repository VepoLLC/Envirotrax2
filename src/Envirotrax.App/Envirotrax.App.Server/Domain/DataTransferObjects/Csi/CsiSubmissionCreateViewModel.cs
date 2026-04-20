using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiSubmissionCreateViewModel
{
    public int SiteId { get; set; }
    public CsiSiteDataDto Site { get; set; } = null!;
    public CsiInspectorDataDto Inspector { get; set; } = null!;
    public CsiLicenseDataDto? CsiLicense { get; set; }
    public List<CsiWaterSupplierOptionDto> AvailableWaterSuppliers { get; set; } = [];
    public int? DefaultWaterSupplierId { get; set; }
    public List<CsiAccountOptionDto> AvailableCsiAccounts { get; set; } = [];
    public int DefaultCsiAccountUserId { get; set; }
}

public class CsiSiteDataDto
{
    public int Id { get; set; }
    public string? AccountNumber { get; set; }
    public string? BusinessName { get; set; }
    public PropertyType PropertyType { get; set; }
    public string? StreetNumber { get; set; }
    public string? StreetName { get; set; }
    public string? PropertyNumber { get; set; }
    public string? City { get; set; }
    public ReferencedStateDto? State { get; set; }
    public string? ZipCode { get; set; }
    public string? MailingCompanyName { get; set; }
    public string? MailingContactName { get; set; }
    public string? MailingStreetNumber { get; set; }
    public string? MailingStreetName { get; set; }
    public string? MailingNumber { get; set; }
    public string? MailingCity { get; set; }
    public ReferencedStateDto? MailingState { get; set; }
    public string? MailingZipCode { get; set; }
    public string? MailingPhoneNumber { get; set; }
    public string? MailingEmailAddress { get; set; }
}

public class CsiInspectorDataDto
{
    public string? CompanyName { get; set; }
    public string? ContactName { get; set; }
    public string? JobTitle { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FaxNumber { get; set; }
    public string? EmailAddress { get; set; }
}

public class CsiLicenseDataDto
{
    public string? LicenseNumber { get; set; }
    public string? LicenseTypeName { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsValid { get; set; }
}

public class CsiAccountOptionDto
{
    public int UserId { get; set; }
    public string? ContactName { get; set; }
    public string? JobTitle { get; set; }
    public string? EmailAddress { get; set; }
    public CsiLicenseDataDto? CsiLicense { get; set; }
}

public class CsiWaterSupplierOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? PwsId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ContactName { get; set; }
    public string? EmailAddress { get; set; }
}
