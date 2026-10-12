using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.Sites;

[Table("RenewalEmailVerifications")]
public class RenewalEmailVerification
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int SiteId { get; set; }
    public Site? Site { get; set; }

    [Required]
    [StringLength(320)]
    public string EmailAddress { get; set; } = null!;

    [Required]
    public string TokenHash { get; set; } = null!;

    public bool IsVerified { get; set; }

    public DateTime? VerifiedDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UnsubscribeTokenHash { get; set; }

    public DateTime? UnsubscribedDate { get; set; }
}
