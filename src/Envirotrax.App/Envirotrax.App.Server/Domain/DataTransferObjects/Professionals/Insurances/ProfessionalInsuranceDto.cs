
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Insurances;

public class ProfessionalInsuranceDto
{
    public int Id { get; set; }

    [Required]
    public ReferencedProfessionalDto Professional { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    [Required]
    [StringLength(50)]
    public string InsuranceNumber { get; set; } = null!;

    public string? FilePath { get; set; }
}

public class CreateInsuranceDto : ProfessionalInsuranceDto
{
    [Required]
    public IFormFile File { get; set; } = null!;
}