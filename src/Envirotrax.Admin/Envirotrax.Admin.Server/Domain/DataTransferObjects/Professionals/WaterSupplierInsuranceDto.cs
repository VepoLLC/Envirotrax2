using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

public class WaterSupplierInsuranceDto
{
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    public DateTime SubmittedOn { get; set; }

    public string? UserEmail { get; set; }

    public string? CompanyName { get; set; }

    public string? ContactName { get; set; }

    public string InsuranceNumber { get; set; } = null!;

    public decimal? CoverageAmount { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public ProfessionalType? ProfessionalType { get; set; }
}
