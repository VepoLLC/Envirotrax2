
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalUserDto : IDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string? ContactName { get; set; }

    [StringLength(150)]
    public string? JobTitle { get; set; }
}