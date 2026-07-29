using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogTripTicketDto : IDto
{
    public int Id { get; set; }

    [StringLength(25)]
    public string? SubmissionId { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    // Site (generator)
    public ReferencedSiteDto? Site { get; set; }

    // Property / Generator
    [StringLength(100)]
    public string? PropertyBusinessName { get; set; }

    public PropertyType PropertyType { get; set; }

    [StringLength(10)]
    public string? PropertyStreetNumber { get; set; }

    [StringLength(100)]
    public string? PropertyStreetName { get; set; }

    [StringLength(50)]
    public string? PropertyNumber { get; set; }

    [StringLength(50)]
    public string? PropertyCity { get; set; }

    public ReferencedStateDto? PropertyState { get; set; }

    [StringLength(50)]
    public string? PropertyZip { get; set; }

    [StringLength(50)]
    public string? FogGeneratorPhoneNumber { get; set; }

    [StringLength(50)]
    public string? FogGeneratorEmailAddress { get; set; }

    [StringLength(50)]
    public string? FogGeneratorContactName { get; set; }

    // Transporter
    public ReferencedProfessionalDto? Professional { get; set; }

    public ReferencedProfessionalUserDto? Transporter { get; set; }

    [StringLength(100)]
    public string? TransporterLicenseNumber { get; set; }

    public DateTime? TransporterLicenseExpiration { get; set; }

    [StringLength(255)]
    public string? TransporterCompanyName { get; set; }

    [StringLength(255)]
    public string? TransporterContactName { get; set; }

    [StringLength(255)]
    public string? TransporterAddress { get; set; }

    [StringLength(255)]
    public string? TransporterCity { get; set; }

    [StringLength(255)]
    public string? TransporterState { get; set; }

    [StringLength(255)]
    public string? TransporterZip { get; set; }

    [StringLength(50)]
    public string? TransporterWorkNumber { get; set; }

    [StringLength(50)]
    public string? TransporterCellNumber { get; set; }

    [StringLength(50)]
    public string? TransporterFaxNumber { get; set; }

    [StringLength(255)]
    public string? TransporterEmailAddress { get; set; }

    [StringLength(500)]
    public string? TransporterSignaturePath { get; set; }

    public DateTime? TransporterSignatureDate { get; set; }

    [StringLength(255)]
    public string? GeneratorContactName { get; set; }

    [StringLength(500)]
    public string? GeneratorSignaturePath { get; set; }

    public DateTime? GeneratorSignatureDate { get; set; }

    // Signature download URLs (generated SAS URLs; not persisted)
    public string? GeneratorSignatureUrl { get; set; }

    public string? ReceiverSignatureUrl { get; set; }

    public string? TransporterSignatureUrl { get; set; }

    // Interceptor
    [StringLength(50)]
    public string? InterceptorType { get; set; }

    [StringLength(100)]
    public string? InterceptorOtherDescription { get; set; }

    public double InterceptorCapacity { get; set; }

    public FogVehicleCapacityType InterceptorCapacityType { get; set; }

    public double InterceptorWasteRemovedAmount { get; set; }

    public FogVehicleCapacityType InterceptorWasteRemovedType { get; set; }

    public double InterceptorWasteRemovedAmountGallons { get; set; }

    public double InterceptorWasteRemovedAmountCubicFeet { get; set; }

    public DateTime? InterceptorWasteRemovedDate { get; set; }

    // Vehicle
    public int? VehicleId { get; set; }

    [StringLength(20)]
    public string? VehicleLicensePlateNumber { get; set; }

    [StringLength(255)]
    public string? VehicleManufacturer { get; set; }

    public int VehicleYear { get; set; }

    public double VehicleCapacity { get; set; }

    public FogVehicleCapacityType VehicleCapacityType { get; set; }

    [StringLength(50)]
    public string? VehicleStickerNumber { get; set; }

    [StringLength(50)]
    public string? VehiclePermitNumber { get; set; }

    // Receiver / Disposal Site
    public int? ReceiverDisposalSiteId { get; set; }

    [StringLength(255)]
    public string? ReceiverCompanyName { get; set; }

    [StringLength(50)]
    public string? ReceiverContactName { get; set; }

    [StringLength(255)]
    public string? ReceiverAddress { get; set; }

    [StringLength(50)]
    public string? ReceiverCity { get; set; }

    [StringLength(50)]
    public string? ReceiverState { get; set; }

    [StringLength(50)]
    public string? ReceiverZip { get; set; }

    [StringLength(50)]
    public string? ReceiverPhoneNumber { get; set; }

    [StringLength(255)]
    public string? ReceiverEmailAddress { get; set; }

    [StringLength(50)]
    public string? ReceiverRegistrationNumber { get; set; }

    [StringLength(50)]
    public string? ReceiverPermitNumber { get; set; }

    public DateTime? ReceiverWasteDeliveredDate { get; set; }

    [StringLength(500)]
    public string? ReceiverSignaturePath { get; set; }

    public DateTime? ReceiverSignatureDate { get; set; }

    // Status
    public bool PickupCompleted { get; set; }

    public bool Completed { get; set; }

    public bool Disapproved { get; set; }

    public DateTime? ApprovalDate { get; set; }

    [StringLength(50)]
    public string? ApprovedBy { get; set; }

    // Validation
    public bool NeedsValidation { get; set; }

    public bool ValidationOnHold { get; set; }

    [StringLength(50)]
    public string? ValidatedBy { get; set; }

    public DateTime? ValidationClearedDate { get; set; }

    public DateTime? ValidationLockedDate { get; set; }

    public bool ValidationNewSite { get; set; }

    public bool ValidationSiteInformationChanged { get; set; }

    public bool ValidationReceiverInformationChanged { get; set; }

    [StringLength(255)]
    public string? ValidationNotes { get; set; }

    // Payment / Transaction
    [StringLength(50)]
    public string? TransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    [Precision(19, 4)]
    public decimal Amount { get; set; }

    [Precision(19, 4)]
    public decimal AmountShare { get; set; }

    public DateTime? WsPaidDate { get; set; }

    public DateTime? SalesRepPaidDate { get; set; }

    // Email
    public bool EmailPdf { get; set; }

    public int EmailStatus { get; set; }

    [StringLength(1000)]
    public string? Comments { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }

    public DateTime? UpdatedTime { get; set; }
}
