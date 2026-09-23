using System.ComponentModel.DataAnnotations;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

public class ReferencedProfessionalUserDto
{
    [Required]
    public int? Id { get; set; }

    public string? EmailAddress { get; set; }

    public string? ContactName { get; set; }
}
