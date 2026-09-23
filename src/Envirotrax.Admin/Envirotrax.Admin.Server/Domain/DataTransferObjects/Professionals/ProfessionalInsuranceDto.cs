using System.ComponentModel.DataAnnotations;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalInsuranceDto
{
    public int Id { get; set; }

    public ReferencedProfessionalDto? Professional { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [Required]
    [StringLength(50)]
    public string InsuranceNumber { get; set; } = null!;

    public string? FilePath { get; set; }

    public ExpirationType ExpirationType { get; set; }
}

public enum ExpirationType
{
    Valid,
    AboutToExpire,
    Expired
}
