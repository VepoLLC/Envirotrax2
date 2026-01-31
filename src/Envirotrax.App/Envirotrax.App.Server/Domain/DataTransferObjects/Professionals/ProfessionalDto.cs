
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalDto : IDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(255)]
    public string? City { get; set; }

    public ReferencedStateDto? State { get; set; }

    [StringLength(25)]
    public string? ZipCode { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    public string? FaxNumber { get; set; }

    [StringLength(255)]
    public string? WebSiteUrl { get; set; }

    public bool HidePublicListing { get; set; }
}

public class ReferencedProfessionalDto
{
    [Required]
    public int? Id { get; set; }

    public string? Name { get; set; }
}