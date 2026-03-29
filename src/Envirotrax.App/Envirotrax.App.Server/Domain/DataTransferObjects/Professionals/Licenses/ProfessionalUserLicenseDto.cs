
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class ProfessionalUserLicenseDto : IDto
{
    public int Id { get; set; }

    [Required]
    public ReferencedProfessionalUserDto User { get; set; } = null!;

    [Required]
    public ProfessionalType? ProfessionalType { get; set; }

    [Required]
    public ReferencedProfessionalLicenseTypeDto LicenseType { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LicenseNumber { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    public ExpirationType ExpirationType { get; set; }
}

public enum ExpirationType
{
    Valid,
    AboutToExpire,
    Expired
}