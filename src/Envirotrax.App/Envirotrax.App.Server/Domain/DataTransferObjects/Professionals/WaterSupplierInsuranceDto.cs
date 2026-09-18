using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class WaterSupplierInsuranceDto : IDto
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyEmail { get; set; }
    public string InsuranceNumber { get; set; } = null!;
    public decimal? InsuranceCoverage { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public ExpirationType ExpirationType { get; set; }
    public ProfessionalType? ProfessionalType { get; set; }
}

public class UpdateWaterSupplierInsuranceDto
{
    [Required]
    [MaxLength(50)]
    public string InsuranceNumber { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    public decimal? InsuranceCoverage { get; set; }
}

public class InsuranceCountsDto
{
    public int UnverifiedCount { get; set; }
    public int ExpiredCount { get; set; }
    public int ExpiringCount { get; set; }
}
