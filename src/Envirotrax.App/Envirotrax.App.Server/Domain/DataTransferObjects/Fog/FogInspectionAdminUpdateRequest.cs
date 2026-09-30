using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogInspectionAdminUpdateRequest
{
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

    public ReferencedStateDto? MailingState { get; set; }

    [StringLength(50)]
    public string? MailingZip { get; set; }

    [StringLength(100)]
    public string? InterceptorType { get; set; }

    [StringLength(200)]
    public string? InterceptorOtherDescription { get; set; }

    public int InterceptorCapacity { get; set; }

    public int InterceptorCapacityType { get; set; }

    [StringLength(200)]
    public string? InterceptorLocationDescription { get; set; }

    public DateTime? InspectionDate { get; set; }

    public FogReasonForInspection ReasonForInspection { get; set; }

    public FacilityType FacilityType { get; set; }

    public bool Maintained { get; set; }

    public bool Accessible { get; set; }

    public bool PastOverflow { get; set; }

    public bool SamplingPointAccessible { get; set; }

    public bool SamplingPointClean { get; set; }

    [StringLength(100)]
    public string? SampledFrom { get; set; }

    public bool InletTeeIntact { get; set; }

    public bool OutletTeeIntact { get; set; }

    [StringLength(50)]
    public string? InletChamberWettingHeight { get; set; }

    [StringLength(50)]
    public string? InletChamberGreaseBlanket { get; set; }

    [StringLength(50)]
    public string? InletChamberSediments { get; set; }

    [StringLength(50)]
    public string? OutletChamberWettingHeight { get; set; }

    [StringLength(50)]
    public string? OutletChamberGreaseBlanket { get; set; }

    [StringLength(50)]
    public string? OutletChamberSediments { get; set; }

    public int InletTotalCapacityPercent { get; set; }

    public int OutletTotalCapacityPercent { get; set; }

    public int TotalCapacityPercent { get; set; }

    public FogInspectionResult InspectionResult { get; set; }

    public string? Comments { get; set; }
}
