
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

public class WaterSupplierDetailsDto
{
    [Required]
    public WaterSupplierDto WaterSupplier { get; set; } = null!;

    [Required]
    public GeneralSettingsDto GeneralSettings { get; set; } = null!;

    [Required]
    public BackflowSettingsDto BackflowSettings { get; set; } = null!;
}
