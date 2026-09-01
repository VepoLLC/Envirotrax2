
using System.ComponentModel.DataAnnotations;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectorAccountDetailsDto
{
    [Required]
    public ProfessionalDto Professional { get; set; } = null!;

    public ProfessionalUserDto? User { get; set; }

    public List<ProfessionalWaterSupplierDto> Registrations { get; set; } = [];
}
