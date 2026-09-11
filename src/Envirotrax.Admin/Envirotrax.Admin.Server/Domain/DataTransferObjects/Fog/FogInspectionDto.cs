using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

public class FogInspectionDto
{
    public int Id { get; set; }

    public DateTime? InspectionDate { get; set; }

    public FogInspectionResult InspectionResult { get; set; }

    public string? TransactionId { get; set; }

    public int TotalCapacityPercent { get; set; }

    public int PropertyType { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public StateDto? PropertyState { get; set; }

    public string? PropertyZip { get; set; }

    public string? InterceptorType { get; set; }

    public string? InterceptorOtherDescription { get; set; }

    public int InterceptorCapacity { get; set; }

    public int InterceptorCapacityType { get; set; }

    public string? InterceptorLocationDescription { get; set; }

    public string? InspectorCompanyName { get; set; }

    public string? InspectorContactName { get; set; }

    public DateTime CreatedTime { get; set; }
}
