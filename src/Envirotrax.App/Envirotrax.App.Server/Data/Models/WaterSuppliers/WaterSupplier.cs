using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class WaterSupplier : TenantBase
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Domain { get; set; } = null!;
}