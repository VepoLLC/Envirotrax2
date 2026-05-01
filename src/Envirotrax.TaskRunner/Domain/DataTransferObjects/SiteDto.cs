
namespace Envirotrax.TaskRunner.Domain.DataTransferObjects;

public class SiteDto
{
    public int Id { get; set; }

    public WaterSupplierDto WaterSupplier { get; set; } = null!;
}