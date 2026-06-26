using Envirotrax.App.Server.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionImageDto : IDto
{
    public int Id { get; set; }
    public int InspectionId { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; } // SAS URL, populated by service — not stored in DB
}
