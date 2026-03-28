
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.Professionals.Licenses;

public class ProfessionalLicenseType
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? Description { get; set; }

    public ProfessionalType ProfessionalType { get; set; }

    public int? StateId { get; set; }
    public State? State { get; set; }
}