
namespace Envirotrax.TaskRunner.Domain.DataTransferObjects;

public class BackflowTestDto
{
    public int Id { get; set; }

    public WaterSupplierDto WaterSupplier { get; set; } = null!;
}
