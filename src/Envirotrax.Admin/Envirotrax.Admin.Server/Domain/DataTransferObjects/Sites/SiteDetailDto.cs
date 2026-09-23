
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

/// <summary>
/// Full read/detail projection of a Site (get-by-id endpoint) that the Admin Edit Site view binds to,
/// mirroring the App SiteDto shape. READ-only — never accepted as an update body. Enum fields are int.
/// </summary>
public class SiteDetailDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public string? SubArea { get; set; }

    public string? AccountNumber { get; set; }

    public string? BusinessName { get; set; }

    public int PropertyType { get; set; }

    public string? StreetNumber { get; set; }

    public string? StreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? City { get; set; }

    public StateDto? State { get; set; }

    public string? ZipCode { get; set; }

    public string? MailingCompanyName { get; set; }

    public string? MailingContactName { get; set; }

    public string? MailingStreetNumber { get; set; }

    public string? MailingStreetName { get; set; }

    public string? MailingNumber { get; set; }

    public string? MailingCity { get; set; }

    public StateDto? MailingState { get; set; }

    public string? MailingZipCode { get; set; }

    public string? MailingPhoneNumber { get; set; }

    public string? MailingEmailAddress { get; set; }

    public string? FogGeneratorPhoneNumber { get; set; }

    public string? FogGeneratorEmailAddress { get; set; }

    public string? Comments { get; set; }

    public bool NeedsCsiInspection { get; set; }

    public DateTime? CsiRenewalDate { get; set; }

    public bool NeedsBackflowLetter { get; set; }

    public DateTime? BackflowLetterDate { get; set; }

    public bool NeedsFogInspection { get; set; }

    public DateTime? FogInspectionExpirationDate { get; set; }

    public bool NeedsFogPermit { get; set; }

    public DateTime? FogPermitExpirationDate { get; set; }

    public DateTime? LastTripTicketDate { get; set; }

    public int TripTicketInterval { get; set; }

    public bool IsFeeExempt { get; set; }

    public int RainFreezeSensorType { get; set; }

    public bool HasKnownBackflowAssemblies { get; set; }

    public bool HasOnSiteSewageFacility { get; set; }

    public bool HasWaterWell { get; set; }

    public bool HasAuxWaterSupply { get; set; }

    public bool HasFireSystem { get; set; }

    public bool FireSeparateWater { get; set; }

    public int GreaseTrapType { get; set; }

    public bool HasGritTrap { get; set; }

    public bool HasReclaimed { get; set; }

    public bool HasIrrigation { get; set; }

    public bool IrrigationSeparateWater { get; set; }

    public bool HasDomesticPremisesIsolation { get; set; }

    public bool RequiresDomesticPremisesIsolation { get; set; }

    public bool InvalidMailingAddress { get; set; }

    public bool OutOfArea { get; set; }

    public int FacilityType { get; set; }

    public int BackflowScheduleMonth { get; set; }

    public double? GisLatitude { get; set; }

    public double? GisLongitude { get; set; }

    public int GisStatus { get; set; }

    public DateTime? GisDate { get; set; }

    public int GisAreaId { get; set; }

    public bool GisOutOfArea { get; set; }

    public DateTime? GisOutOfAreaCheckDate { get; set; }

    public string? CustomData1 { get; set; }

    public bool CustomBooleanData1 { get; set; }

    public bool BypassPropertyNumberValidation { get; set; }

    public bool NeedsRenewalCheck { get; set; }

    public bool Active { get; set; }

    public DateTime CreatedTime { get; set; }

    public DateTime? UpdatedTime { get; set; }

    public SiteUpdatedByDto? UpdatedBy { get; set; }
}

/// <summary>
/// The user who last modified the site. Mirrors the App AppUserDto shape (Id + Email only — V2 does
/// not expose a display name). Read-only; used for the "Last Modified By" display.
/// </summary>
public class SiteUpdatedByDto
{
    public int Id { get; set; }

    public string? Email { get; set; }
}
