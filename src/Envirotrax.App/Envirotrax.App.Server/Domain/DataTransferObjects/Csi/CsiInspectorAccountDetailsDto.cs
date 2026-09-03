
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

/// <summary>
/// Everything the admin dashboard shows in the CSI inspector details window: the company (professional)
/// account, the sub account that was clicked in the search results, and every water supplier registration
/// the company holds.
/// </summary>
public class CsiInspectorAccountDetailsDto
{
    [Required]
    public ProfessionalDto Professional { get; set; } = null!;

    public ProfessionalUserDto? User { get; set; }

    public List<ProfessionalWaterSupplierDto> Registrations { get; set; } = [];
}
