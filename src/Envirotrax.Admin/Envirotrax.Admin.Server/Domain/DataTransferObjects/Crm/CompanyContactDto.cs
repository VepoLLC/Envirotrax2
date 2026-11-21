using System.ComponentModel.DataAnnotations;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Crm;

/// <summary>
/// Mirrors the App's CompanyContactDto, validation included, so a bad save is rejected here before the proxy hop.
/// </summary>
public class CompanyContactDto
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

    public StateDto? State { get; set; }

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
