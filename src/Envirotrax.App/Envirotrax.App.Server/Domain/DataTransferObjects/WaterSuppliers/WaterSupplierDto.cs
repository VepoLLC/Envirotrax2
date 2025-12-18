
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers
{
    public class WaterSupplierDto : IDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Domain { get; set; } = null!;

        public ReferencedWaterSupplierDto? Parent { get; set; }
    }

    public class ReferencedWaterSupplierDto
    {
        [Required]
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Domain { get; set; }
    }
}