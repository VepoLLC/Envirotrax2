
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Auth.Data.Models;

public class UserInvitation : ICreateAuditableModel<AppUser>
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public AppUser? User { get; set; }

    [Required]
    public string TokenHash { get; set; } = null!;

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}