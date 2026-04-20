
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class WaterSupplierUserDto : IDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string ContactName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string EmailAddress { get; set; } = null!;
}

public class ReferencedWaterSupplierUserDto
{
    [Required]
    public int? Id { get; set; }

    public string? ContactName { get; set; }

    public string? EmailAddress { get; set; }
}