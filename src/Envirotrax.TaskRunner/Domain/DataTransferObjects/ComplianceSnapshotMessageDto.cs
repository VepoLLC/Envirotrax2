
namespace Envirotrax.TaskRunner.Domain.DataTransferObjects;

public class ComplianceSnapshotMessageDto
{
    public WaterSupplierDto WaterSupplier { get; set; } = null!;

    public DateTime ReportDate { get; set; }
}
