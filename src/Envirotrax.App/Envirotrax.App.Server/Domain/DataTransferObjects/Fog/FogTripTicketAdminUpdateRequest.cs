using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// The fields the Admin FOG Trip Ticket Details window edits: what the transporter typed on the submission form.
/// The transporter, vehicle and receiver blocks are snapshots taken at submit time and stay read-only.
/// </summary>
public class FogTripTicketAdminUpdateRequest
{
    // Generator
    public PropertyType PropertyType { get; set; }

    [StringLength(100)]
    public string? PropertyBusinessName { get; set; }

    [StringLength(50)]
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

    [StringLength(255)]
    public string? GeneratorContactName { get; set; }

    // Interceptor
    [StringLength(50)]
    public string? InterceptorType { get; set; }

    [StringLength(100)]
    public string? InterceptorOtherDescription { get; set; }

    public double InterceptorCapacity { get; set; }

    public FogVehicleCapacityType InterceptorCapacityType { get; set; }

    public double InterceptorWasteRemovedAmount { get; set; }

    public FogVehicleCapacityType InterceptorWasteRemovedType { get; set; }

    public DateTime? InterceptorWasteRemovedDate { get; set; }

    // Receiver
    [StringLength(50)]
    public string? ReceiverContactName { get; set; }

    public DateTime? ReceiverWasteDeliveredDate { get; set; }

    [StringLength(1000)]
    public string? Comments { get; set; }
}
