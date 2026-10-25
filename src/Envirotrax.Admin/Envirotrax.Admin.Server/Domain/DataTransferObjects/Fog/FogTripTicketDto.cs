using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

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

    public DateTime? ReceiverWasteDeliveredDate { get; set; }

    public bool PickupCompleted { get; set; }

    public bool Completed { get; set; }

    public string? TransactionId { get; set; }

    public DateTime CreatedTime { get; set; }
}
