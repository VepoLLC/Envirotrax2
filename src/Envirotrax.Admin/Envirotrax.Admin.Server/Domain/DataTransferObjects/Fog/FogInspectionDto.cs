using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// Mirror of the App's FogInspectionDto. One shape serves both the search grid and the details window: the
/// search response simply leaves the detail members null. This is a plain deserialization target for the proxy
/// - no query or filter is built from it here - so a separate details DTO would be duplication, not separation.
/// </summary>
public class FogInspectionDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public DateTime? InspectionDate { get; set; }

    public string? SubmissionId { get; set; }

    public FogInspectionResult InspectionResult { get; set; }

    public string? TransactionId { get; set; }

    public int TotalCapacityPercent { get; set; }

    // Property
    public int PropertyType { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public StateDto? PropertyState { get; set; }

    public string? PropertyZip { get; set; }

    // Mailing
    public string? MailingCompanyName { get; set; }

    public string? MailingContactName { get; set; }

    public string? MailingStreetNumber { get; set; }

    public string? MailingStreetName { get; set; }

    public string? MailingNumber { get; set; }

    public string? MailingCity { get; set; }

    public StateDto? MailingState { get; set; }

    public string? MailingZip { get; set; }

    public string? MailingPhoneNumber { get; set; }

    public string? MailingEmailAddress { get; set; }

    // Inspector
    public ReferencedProfessionalUserDto? Inspector { get; set; }

    public string? InspectorCompanyName { get; set; }

    public string? InspectorJobTitle { get; set; }

    public string? InspectorContactName { get; set; }

    public string? InspectorAddress { get; set; }

    public string? InspectorCity { get; set; }

    public string? InspectorState { get; set; }

    public string? InspectorZip { get; set; }

    public string? InspectorWorkNumber { get; set; }

    public string? InspectorCellNumber { get; set; }

    public string? InspectorFaxNumber { get; set; }

    // Generator contact
    public string? FogGeneratorPhoneNumber { get; set; }

    public string? FogGeneratorEmailAddress { get; set; }

    public int FacilityType { get; set; }

    public int ReasonForInspection { get; set; }

    // Trap / interceptor
    public string? InterceptorType { get; set; }

    public string? InterceptorOtherDescription { get; set; }

    public int InterceptorCapacity { get; set; }

    public int InterceptorCapacityType { get; set; }

    public string? InterceptorLocationDescription { get; set; }

    public string? InterceptorComments { get; set; }

    // Inspection results
    public bool Maintained { get; set; }

    public bool Accessible { get; set; }

    public bool PastOverflow { get; set; }

    public string? InletChamberWettingHeight { get; set; }

    public string? InletChamberGreaseBlanket { get; set; }

    public string? InletChamberSediments { get; set; }

    public string? OutletChamberWettingHeight { get; set; }

    public string? OutletChamberGreaseBlanket { get; set; }

    public string? OutletChamberSediments { get; set; }

    public bool InletTeeIntact { get; set; }

    public bool OutletTeeIntact { get; set; }

    public bool? InletTeeVisible { get; set; }

    public bool? OutletTeeVisible { get; set; }

    public string? SampledFrom { get; set; }

    public bool SamplingPointAccessible { get; set; }

    public bool SamplingPointClean { get; set; }

    public int InletTotalCapacityPercent { get; set; }

    public int OutletTotalCapacityPercent { get; set; }

    // Signature / remarks
    public string? SignatureContactName { get; set; }

    public DateTime? SignatureDate { get; set; }

    public string? Comments { get; set; }

    // Images arrive as ready-to-use SAS URLs from the App's FogInspectionService.GetAsync override.
    public string? ExteriorImageUrl { get; set; }

    public string? InteriorImageUrl { get; set; }

    public string? SignatureImageUrl { get; set; }

    public DateTime CreatedTime { get; set; }
}
