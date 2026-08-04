using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

/// <summary>
/// Admin Edit Site update payload — only the approved editable fields (Property Info, Mailing Info,
/// Property Settings). Never add Id/WaterSupplierId, audit, assignment ids, GIS, import fields, or
/// NeedsRenewalCheck; GIS is saved via its own endpoint and NeedsRenewalCheck is derived server-side.
/// </summary>
public class AdminUpdateSiteDto
{
    // --- Property Information ---
    public PropertyType PropertyType { get; set; }

    [StringLength(100)]
    public string? BusinessName { get; set; }

    [StringLength(50)]
    public string? StreetNumber { get; set; }

    [StringLength(100)]
    public string? StreetName { get; set; }

    [StringLength(50)]
    public string? PropertyNumber { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    public int? StateId { get; set; }

    [StringLength(50)]
    public string? ZipCode { get; set; }

    // --- Mailing Information ---
    [StringLength(100)]
    public string? MailingCompanyName { get; set; }

    [StringLength(100)]
    public string? MailingContactName { get; set; }

    [StringLength(50)]
    public string? MailingStreetNumber { get; set; }

    [StringLength(100)]
    public string? MailingStreetName { get; set; }

    [StringLength(50)]
    public string? MailingNumber { get; set; }

    [StringLength(50)]
    public string? MailingCity { get; set; }

    public int? MailingStateId { get; set; }

    [StringLength(50)]
    public string? MailingZipCode { get; set; }

    [StringLength(50)]
    public string? MailingPhoneNumber { get; set; }

    [StringLength(100)]
    public string? MailingEmailAddress { get; set; }

    // --- Property Settings ---
    [Required]
    [StringLength(20)]
    public string AccountNumber { get; set; } = null!;

    public bool Active { get; set; }

    public bool InvalidMailingAddress { get; set; }

    public bool OutOfArea { get; set; }

    public bool IsFeeExempt { get; set; }

    public bool BypassPropertyNumberValidation { get; set; }

    public int BackflowScheduleMonth { get; set; }

    public bool NeedsCsiInspection { get; set; }

    public DateTime? CsiRenewalDate { get; set; }

    public bool NeedsFogInspection { get; set; }

    public DateTime? FogInspectionExpirationDate { get; set; }

    public bool NeedsFogPermit { get; set; }

    public DateTime? FogPermitExpirationDate { get; set; }

    public DateTime? LastTripTicketDate { get; set; }

    public int TripTicketInterval { get; set; }

    public FacilityType FacilityType { get; set; }

    public GreaseTrapType GreaseTrapType { get; set; }

    public bool HasOnSiteSewageFacility { get; set; }

    public bool HasAuxWaterSupply { get; set; }

    public bool HasFireSystem { get; set; }

    public bool FireSeparateWater { get; set; }

    public bool HasGritTrap { get; set; }

    public bool HasIrrigation { get; set; }

    public bool IrrigationSeparateWater { get; set; }

    public bool HasDomesticPremisesIsolation { get; set; }
}
