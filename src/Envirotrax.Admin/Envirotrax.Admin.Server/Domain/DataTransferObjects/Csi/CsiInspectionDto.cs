
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionDto
{
    public int Id { get; set; }

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
}
