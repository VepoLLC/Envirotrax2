
namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionImageDto
{
    public int Id { get; set; }

    public int InspectionId { get; set; }

    public string? Description { get; set; }

    public string? Url { get; set; }
}
