using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowOutOfServiceRequestDto : IDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public ReferencedProfessionalDto? Professional { get; set; }

    public int? BpatId { get; set; }

    public int TestId { get; set; }

    public BackflowTestDto? Test { get; set; }

    public OutOfServiceType Type { get; set; }

    public string? Description { get; set; }

    public int? ReplacementAssemblyTestId { get; set; }

    public BackflowTestDto? ReplacementAssemblyTest { get; set; }

    public DateTime? OutOfServiceDate { get; set; }

    public DateTime? ClearedDate { get; set; }
}
