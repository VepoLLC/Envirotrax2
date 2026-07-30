
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

    [StringLength(25)]
    public string? CellNumber { get; set; }
}

public class ReferencedWaterSupplierUserDto
{
    [Required]
    public int? Id { get; set; }

    public string? ContactName { get; set; }

    public string? EmailAddress { get; set; }
}