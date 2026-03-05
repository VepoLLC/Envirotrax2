
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class AppUserDto : IDto
{
    public int Id { get; set; }

    public string? Email { get; set; }
}

public class ReferencedAppUserDto
{
    [Required]
    public int? Id { get; set; }
}