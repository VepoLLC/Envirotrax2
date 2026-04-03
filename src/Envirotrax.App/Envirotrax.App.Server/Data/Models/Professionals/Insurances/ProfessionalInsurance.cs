
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Professionals.Insurances;

public class ProfessionalInsurance : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [Required]
    [StringLength(50)]
    public string InsuranceNumber { get; set; } = null!;

    [Required]
    public string FilePath { get; set; } = null!;

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}