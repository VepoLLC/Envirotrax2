using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

public class FogTripTicketUpdateRequest
{
    // Generator
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

    // Interceptor
    public string? InterceptorType { get; set; }

    public string? InterceptorOtherDescription { get; set; }

    public double InterceptorCapacity { get; set; }

    public int InterceptorCapacityType { get; set; }

    public double InterceptorWasteRemovedAmount { get; set; }

    public int InterceptorWasteRemovedType { get; set; }

    public DateTime? InterceptorWasteRemovedDate { get; set; }

    // Receiver
    public string? ReceiverContactName { get; set; }

    public DateTime? ReceiverWasteDeliveredDate { get; set; }

    public string? Comments { get; set; }
}
