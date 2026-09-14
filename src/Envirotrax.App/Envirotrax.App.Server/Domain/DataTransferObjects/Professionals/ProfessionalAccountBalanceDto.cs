
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalAccountBalanceDto
{
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal AmountToAdd { get; set; }

    [Required]
    public string DataDescriptor { get; set; } = null!;

    [Required]
    public string DataValue { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string BillingFirstName { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string BillingLastName { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string BillingAddress { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string BillingCity { get; set; } = null!;

    [Required]
    public ReferencedStateDto BillingState { get; set; } = null!;

    [Required]
    [StringLength(25)]
    public string BillingZipCode { get; set; } = null!;
}
