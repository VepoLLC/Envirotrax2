
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers
{
    public class WaterSupplierDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Domain { get; set; } = null!;
    }
}