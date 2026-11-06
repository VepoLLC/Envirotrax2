using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// Mirror of the App's FogTripTicketDto. One shape serves the search grid and the details window; only the
/// details GET fills the signature URLs.
/// </summary>
public class FogTripTicketDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public ReferencedSiteDto? Site { get; set; }

    // Generator / property
    public int PropertyType { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public StateDto? PropertyState { get; set; }

    public string? PropertyZip { get; set; }

    public string? FogGeneratorPhoneNumber { get; set; }

    public string? FogGeneratorEmailAddress { get; set; }

    public string? GeneratorContactName { get; set; }

    // Signatures arrive as ready-to-use SAS URLs from the App's FogTripTicketService.GetAsync override.
    public string? GeneratorSignatureUrl { get; set; }

    public string? TransporterSignatureUrl { get; set; }

    public string? ReceiverSignatureUrl { get; set; }

    // Transporter
    public ReferencedProfessionalDto? Professional { get; set; }

    public ReferencedProfessionalUserDto? Transporter { get; set; }

    public string? TransporterCompanyName { get; set; }

    public string? TransporterContactName { get; set; }

    public string? TransporterAddress { get; set; }

    public string? TransporterCity { get; set; }

    public string? TransporterState { get; set; }

    public string? TransporterZip { get; set; }

    public string? TransporterLicenseNumber { get; set; }

    public string? TransporterEmailAddress { get; set; }

    public string? TransporterWorkNumber { get; set; }

    // Vehicle
    public string? VehicleLicensePlateNumber { get; set; }

    public string? VehicleManufacturer { get; set; }

    public int VehicleYear { get; set; }

    public double VehicleCapacity { get; set; }

    public int VehicleCapacityType { get; set; }

    public string? VehicleStickerNumber { get; set; }

    // Interceptor / waste
    public string? InterceptorType { get; set; }

    public string? InterceptorOtherDescription { get; set; }

    public double InterceptorCapacity { get; set; }

    public int InterceptorCapacityType { get; set; }

    public double InterceptorWasteRemovedAmount { get; set; }

    public int InterceptorWasteRemovedType { get; set; }

    public DateTime? InterceptorWasteRemovedDate { get; set; }

    // Receiver
    public string? ReceiverCompanyName { get; set; }

    public string? ReceiverContactName { get; set; }

    public string? ReceiverAddress { get; set; }

    public string? ReceiverCity { get; set; }

    public string? ReceiverState { get; set; }

    public string? ReceiverZip { get; set; }

    public string? ReceiverRegistrationNumber { get; set; }

    public string? ReceiverPhoneNumber { get; set; }

    public string? ReceiverEmailAddress { get; set; }

    public DateTime? ReceiverWasteDeliveredDate { get; set; }

    public string? Comments { get; set; }

    public bool PickupCompleted { get; set; }

    public bool Completed { get; set; }

    public string? TransactionId { get; set; }

    // Validation
    public bool NeedsValidation { get; set; }

    public bool ValidationOnHold { get; set; }

    public bool ValidationNewSite { get; set; }

    public bool ValidationSiteInformationChanged { get; set; }

    public bool ValidationReceiverInformationChanged { get; set; }

    public DateTime CreatedTime { get; set; }
}
