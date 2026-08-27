using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogVehiclePermitDto : IDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PermitNumber { get; set; } = null!;

    public DateTime? InspectionDueDate { get; set; }

    public bool IsActive { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }
}
