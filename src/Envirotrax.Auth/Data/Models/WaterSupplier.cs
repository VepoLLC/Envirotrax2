
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Auth.Data.Models
{
    [ReadOnlyModel]
    [ExcludedModel]
    public class WaterSupplier : TenantBase
    {
        public const string EnvirotraxAdminDomain = "vepollc";

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Domain { get; set; } = null!;

        [ForeignKey(nameof(ParentId))]
        public WaterSupplier? Parent { get; set; }
    }
}