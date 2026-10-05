
namespace Envirotrax.App.Server.Data.Models.PublicSearch;

public class PublicCsiInspectionResult
{
    public int Id { get; set; }

    public DateTime? InspectionDate { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public string? PropertyState { get; set; }

    public string? PropertyZip { get; set; }

    public string? InspectorCompanyName { get; set; }

    public string? InspectorContactName { get; set; }

    public string? InspectorAddress { get; set; }

    public string? InspectorCity { get; set; }

    public string? InspectorState { get; set; }

    public string? InspectorZip { get; set; }
}
