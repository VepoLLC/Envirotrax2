
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public DateTime? InspectionDate { get; set; }

    public bool InspectionResult { get; set; }

    public string? TransactionId { get; set; }

    public int PropertyType { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public StateDto? PropertyState { get; set; }

    public string? PropertyZip { get; set; }

    public string? InspectorCompanyName { get; set; }

    public string? InspectorContactName { get; set; }

    public string? InspectorLicenseType { get; set; }

    public string? InspectorLicenseNumber { get; set; }

    // Validation
    public bool NeedsValidation { get; set; }

    public bool ValidationNewSite { get; set; }

    public bool ValidationSiteInformationChanged { get; set; }

    public bool ValidationOnHold { get; set; }

    public DateTime CreatedTime { get; set; }
}
