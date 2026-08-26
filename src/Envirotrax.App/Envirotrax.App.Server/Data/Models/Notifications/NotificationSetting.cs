using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Notifications;

[Table("NotificationSettings")]
public class NotificationSetting : TenantModel<WaterSupplier>, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int UserId { get; set; }
    public WaterSupplierUser? User { get; set; }

    [Required]
    [StringLength(50)]
    public string Description { get; set; } = null!;

    [Required]
    [StringLength(7)]
    public string Color { get; set; } = null!;

    public BackflowReasonForTest? ReasonForTest { get; set; }

    public bool PropertyTypeResidential { get; set; }
    public bool PropertyTypeCommercial { get; set; }
    public bool PropertyTypeAny { get; set; }

    public bool FilterFailedTest { get; set; }
    public bool FilterPassingTest { get; set; }
    public bool FilterUnknownSerialNumber { get; set; }
    public bool FilterInactiveProperty { get; set; }
    public bool FilterNonCompliance { get; set; }
    public bool FilterPotableNonPotableMismatch { get; set; }
    public bool FilterDuplicateTest { get; set; }
    public bool FilterOutOfService { get; set; }
    public bool FilterContainsRemarks { get; set; }
    public bool FilterBackflowNotProperlyInstalled { get; set; }
    public bool FilterFeeExempt { get; set; }
    public bool FilterHasOnSiteSewageFacility { get; set; }
    public bool FilterHasAuxWaterSupply { get; set; }
    public bool FilterSubmissionDaysExceeded { get; set; }
    public int FilterSubmissionDaysExceededDays { get; set; }
    public bool FilterAny { get; set; }

    public bool HazardTypeAgriculturalFeedLot { get; set; }
    public bool HazardTypeDomesticPremisesIsolation { get; set; }
    public bool HazardTypeFireSystem { get; set; }
    public bool HazardTypeFireHydrantTemporaryConstruction { get; set; }
    public bool HazardTypeGasStationCarWash { get; set; }
    public bool HazardTypeIrrigationNonChemical { get; set; }
    public bool HazardTypeIrrigationChemicalFeed { get; set; }
    public bool HazardTypeLaundryCleaners { get; set; }
    public bool HazardTypeMedicalDentalLaboratoryMortuary { get; set; }
    public bool HazardTypeNailsSalonGrooming { get; set; }
    public bool HazardTypePoolRecreationAthletics { get; set; }
    public bool HazardTypeRestaurantVendingGrocery { get; set; }
    public bool HazardTypeFountainsGardenPondsWaterFeatures { get; set; }
    public bool HazardTypeWaterSoftener { get; set; }
    public bool HazardTypeOther { get; set; }
    public bool HazardTypeAny { get; set; }

    public NotificationInterval Interval { get; set; }
    public NotificationDeliveryType DeliveryType { get; set; }

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}
