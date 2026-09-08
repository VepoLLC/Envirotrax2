
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTesterAccountDto
{
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    public string? EmailAddress { get; set; }

    public string? CompanyName { get; set; }

    public string? ContactName { get; set; }

    public string? JobTitle { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public StateDto? State { get; set; }

    public string? ZipCode { get; set; }

    public string? WorkNumber { get; set; }

    public string? CellNumber { get; set; }

    public bool IsSubAccount { get; set; }
}
