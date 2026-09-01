using System.ComponentModel.DataAnnotations;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class ProfessionalLicenseTypeDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? Description { get; set; }

    public ProfessionalType ProfessionalType { get; set; }

    public StateDto? State { get; set; }
}

public class ReferencedProfessionalLicenseTypeDto
{
    [Required]
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public ProfessionalType ProfessionalType { get; set; }

    public StateDto? State { get; set; }
}
