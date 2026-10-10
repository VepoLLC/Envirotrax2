using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class RenewalOptInSiteDto
{
    public bool Found { get; set; }

    public RenewalOptInType OptInType { get; set; }
}

public class RenewalOptInRequestDto
{
    [Required]
    [StringLength(20)]
    public string Passcode { get; set; } = null!;

    [Required]
    [StringLength(3)]
    public string ZipCodePrefix { get; set; } = null!;

    public RenewalOptInType OptInType { get; set; }

    [StringLength(500)]
    public string? EmailAddresses { get; set; }
}

public class RenewalOptInTokenDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Token { get; set; } = null!;
}

public class RenewalOptInResultDto
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = null!;
}
