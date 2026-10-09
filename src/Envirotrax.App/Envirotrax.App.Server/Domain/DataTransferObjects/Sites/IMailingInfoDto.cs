using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

/// <summary>
/// The property owner's mailing/contact fields: kept on a site and snapshotted onto each backflow test,
/// CSI inspection and FOG inspection.
/// </summary>
public interface IMailingInfoDto
{
    string? MailingCompanyName { get; set; }
    string? MailingContactName { get; set; }
    string? MailingStreetNumber { get; set; }
    string? MailingStreetName { get; set; }
    string? MailingNumber { get; set; }
    string? MailingCity { get; set; }
    ReferencedStateDto? MailingState { get; set; }
    string? MailingZip { get; set; }
    string? MailingPhoneNumber { get; set; }
    string? MailingEmailAddress { get; set; }
}

/// <summary>
/// A record shown to professionals that belongs to one water supplier and carries owner mailing info,
/// either its own snapshot, its site's, or both, so it can be redacted in one pass.
/// </summary>
public interface IRedactableMailingInfoDto
{
    ReferencedWaterSupplierDto? WaterSupplier { get; }

    IEnumerable<IMailingInfoDto?> MailingInfo { get; }
}
