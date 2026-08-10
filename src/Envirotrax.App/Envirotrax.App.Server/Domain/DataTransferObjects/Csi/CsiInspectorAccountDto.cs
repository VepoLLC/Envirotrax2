
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectorAccountDto
{
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    public bool IsMasterAccount { get; set; }

    public string? EmailAddress { get; set; }

    public string? CompanyName { get; set; }

    public string? ContactName { get; set; }

    public string? JobTitle { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public ReferencedStateDto? State { get; set; }

    public string? ZipCode { get; set; }

    public string? WorkNumber { get; set; }
}
