
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? CompanyEmail { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public StateDto? State { get; set; }

    public string? ZipCode { get; set; }

    public string? PhoneNumber { get; set; }
}
