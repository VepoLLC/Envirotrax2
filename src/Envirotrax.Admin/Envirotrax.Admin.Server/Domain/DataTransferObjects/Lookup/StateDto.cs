using System.ComponentModel.DataAnnotations;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup
{
    public class StateDto
    {
        [Required]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
