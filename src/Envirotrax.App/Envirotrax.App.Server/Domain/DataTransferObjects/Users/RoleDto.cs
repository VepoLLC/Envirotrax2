
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users
{
    public class RoleDto : IDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DeletedTime { get; set; }
    }

    public class ReferencedRoleDto
    {
        [Required]
        public int? Id { get; set; }

        public string? Name { get; set; }
    }
}