using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class UpdateWaterSupplierLicenseDto
{
    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = null!;

    [MaxLength(255)]
    public string? ContactName { get; set; }

    public DateTime? ExpirationDate { get; set; }
}
