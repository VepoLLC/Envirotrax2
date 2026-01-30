using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Sites;

[Table("Sites")]
public class Site : TenantModel<WaterSupplier>, IAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    [StringLength(10)]
    public string? SubArea { get; set; }

    [Required]
    [StringLength(20)]
    public string AccountNumber { get; set; } = null!;

    [StringLength(100)]
    public string? BusinessName { get; set; }

    public PropertyType? PropertyType { get; set; }

    [StringLength(50)]
    public string? StreetNumber { get; set; }

    [StringLength(100)]
    public string? StreetName { get; set; }

    [StringLength(50)]
    public string? PropertyNumber { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    public int? StateId { get; set; }
    public State? State { get; set; }

    [StringLength(50)]
    public string? ZipCode { get; set; }

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
    public State? MailingState { get; set; }

    [StringLength(50)]
    public string? MailingZipCode { get; set; }

    [StringLength(50)]
    public string? MailingPhoneNumber { get; set; }

    [StringLength(100)]
    public string? MailingEmailAddress { get; set; }

    [StringLength(50)]
    public string? FogGeneratorPhoneNumber { get; set; }

    [StringLength(50)]
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

    public int HasGreaseTrap { get; set; }

    public bool HasGritTrap { get; set; }

    public bool HasReclaimed { get; set; }

    public bool HasIrrigation { get; set; }

    public bool IrrigationSeparateWater { get; set; }

    public bool HasDomesticPremisesIsolation { get; set; }

    public bool RequiresDomesticPremisesIsolation { get; set; }

    public bool InvalidMailingAddress { get; set; }

    public bool OutOfArea { get; set; }

    [StringLength(50)]
    public string? FacilityType { get; set; }

    public byte[]? FacilityMap { get; set; }

    public int BackflowScheduleMonth { get; set; }

    public double? GisLatitude { get; set; }

    public double? GisLongitude { get; set; }

    public int GisStatus { get; set; }

    [Column(TypeName = "date")]
    public DateTime? GisDate { get; set; }

    public int GisAreaId { get; set; }

    public bool GisOutOfArea { get; set; }

    public DateTime? GisOutOfAreaCheckDate { get; set; }

    [StringLength(50)]
    public string? ImportSiteId { get; set; }

    [StringLength(50)]
    public string? ImportSiteId2 { get; set; }

    public int ImportId { get; set; }

    public bool ExcludeFromBackflowMailing { get; set; }

    public bool ExcludeFromCsiMailing { get; set; }

    public bool NeedsValidation { get; set; }

    public bool ValidationOnHold { get; set; }

    public bool BypassPropertyNumberValidation { get; set; }

    public int UnknownAssemblyLettersSent { get; set; }

    public int UnknownAssembliesLetterCount { get; set; }

    public DateTime? UnknownAssembliesLetterStartDate { get; set; }

    [StringLength(20)]
    public string? CustomData1 { get; set; }

    public bool CustomBooleanData1 { get; set; }

    [StringLength(50)]
    public string? UserAccountAssignment { get; set; }

    [StringLength(50)]
    public string? CsiAccountAssignment { get; set; }

    [StringLength(50)]
    public string? BackflowAccountAssignment { get; set; }

    [StringLength(50)]
    public string? FogAccountAssignment { get; set; }

    public bool NeedsRenewalCheck { get; set; }

    public DateTime? CsiAccountAssignmentDate { get; set; }

    public DateTime? BackflowAccountAssignmentDate { get; set; }

    public DateTime? FogAccountAssignmentDate { get; set; }

    // Audit properties
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? UpdatedById { get; set; }
    public AppUser? UpdatedBy { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}
