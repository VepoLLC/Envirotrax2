using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

public class FogInspectionUpdateRequest
{
    public int PropertyType { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public StateDto? PropertyState { get; set; }

    public string? PropertyZip { get; set; }

    public string? MailingCompanyName { get; set; }

    public string? MailingContactName { get; set; }

    public string? MailingStreetNumber { get; set; }

    public string? MailingStreetName { get; set; }

    public string? MailingNumber { get; set; }

    public string? MailingCity { get; set; }

    public StateDto? MailingState { get; set; }

    public string? MailingZip { get; set; }

    public string? InterceptorType { get; set; }

    public string? InterceptorOtherDescription { get; set; }

    public int InterceptorCapacity { get; set; }

    public int InterceptorCapacityType { get; set; }

    public string? InterceptorLocationDescription { get; set; }

    public DateTime? InspectionDate { get; set; }

    public int ReasonForInspection { get; set; }

    public int FacilityType { get; set; }

    public bool Maintained { get; set; }

    public bool Accessible { get; set; }

    public bool PastOverflow { get; set; }

    public bool SamplingPointAccessible { get; set; }

    public bool SamplingPointClean { get; set; }

    public string? SampledFrom { get; set; }

    public bool InletTeeIntact { get; set; }

    public bool OutletTeeIntact { get; set; }

    public string? InletChamberWettingHeight { get; set; }

    public string? InletChamberGreaseBlanket { get; set; }

    public string? InletChamberSediments { get; set; }

    public string? OutletChamberWettingHeight { get; set; }

    public string? OutletChamberGreaseBlanket { get; set; }

    public string? OutletChamberSediments { get; set; }

    public int InletTotalCapacityPercent { get; set; }

    public int OutletTotalCapacityPercent { get; set; }

    public int TotalCapacityPercent { get; set; }

    public int InspectionResult { get; set; }

    public string? Comments { get; set; }
}
