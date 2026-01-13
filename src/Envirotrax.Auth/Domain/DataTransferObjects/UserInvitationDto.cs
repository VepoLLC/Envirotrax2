
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.Auth.Domain.DataTransferObjects;

public class UserInvitationDto
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string EmailAddress { get; set; } = null!;
}