using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class WaterSupplierInsuranceDto : IDto
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public DateTime SubmittedOn { get; set; }
    public string? UserEmail { get; set; }
    public string? CompanyName { get; set; }
    public string? ContactName { get; set; }
    public string InsuranceNumber { get; set; } = null!;
    public DateTime? ExpirationDate { get; set; }
    public ProfessionalType? ProfessionalType { get; set; }
}
