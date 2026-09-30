using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

public class CreditCardPaymentDto
{
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
