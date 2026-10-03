
namespace Envirotrax.App.Server.Domain.DataTransferObjects.PublicSearch;

public class PublicSearchCriteriaDto
{
    public int WaterSupplierId { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }
}
