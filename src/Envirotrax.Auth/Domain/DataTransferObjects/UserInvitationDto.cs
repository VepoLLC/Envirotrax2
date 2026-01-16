
using System.ComponentModel.DataAnnotations;
using Envirotrax.Auth.Data.Models;

namespace Envirotrax.Auth.Domain.DataTransferObjects;

public class UserInvitationDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    public int? CreatorId { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string EmailAddress { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string InvitedByCompany { get; set; } = null!;
}

public class InvitationValidationResultDto
{
    public bool IsValid { get; set; }

    public AppUser? User { get; set; }
}