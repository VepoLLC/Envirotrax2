
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Models.Professionals;

public class ProfessionalInsurance : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public DateTime? ExpirationDate { get; set; }

    // Both of these are transcribed off the certificate by water supplier staff, never by the
    // contractor. Null means the policy has not been validated yet, which is what puts it in the
    // Insurance Management queue and blocks the contractor from submitting work.
    [Precision(19, 4)]
    public decimal? InsuranceCoverage { get; set; }

    [Required]
    [StringLength(50)]
    public string InsuranceNumber { get; set; } = null!;

    [Required]
    public string FilePath { get; set; } = null!;

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}