
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class CreateProfessionalDto
{
    [Required]
    public ProfessionalDto Professional { get; set; } = null!;

    [Required]
    public ProfessionalUserDto User { get; set; } = null!;
}