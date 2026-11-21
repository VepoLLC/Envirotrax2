using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Crm;

/// <summary>
/// One of a CRM company's additional contacts. The company comes from the route, so it is not on the DTO.
/// </summary>
public class CompanyContactDto : IDto
{
    public int Id { get; set; }

    [StringLength(100)]
    public string? Position { get; set; }

    [StringLength(100)]
    public string? ContactName { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    public ReferencedStateDto? State { get; set; }

    [StringLength(50)]
    public string? ZipCode { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    public string? CellNumber { get; set; }

    [StringLength(100)]
    public string? EmailAddress { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
}
