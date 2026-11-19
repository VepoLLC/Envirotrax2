using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Crm;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Crm;

public class CompanyDto : IDto
{
    public int Id { get; set; }

    public ContactType ContactType { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

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

    [StringLength(100)]
    public string? EmailAddress { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    public string? CellNumber { get; set; }

    public bool HasBackflowTesting { get; set; }
    public DateTime? BackflowTestingStartDate { get; set; }

    public bool HasCsiInspection { get; set; }
    public DateTime? CsiInspectionStartDate { get; set; }

    public bool HasFogInspection { get; set; }
    public DateTime? FogInspectionStartDate { get; set; }

    public bool HasFogTransportation { get; set; }
    public DateTime? FogTransportationStartDate { get; set; }

    public AppUserDto? LastContactedBy { get; set; }

    public DateTime? LastContactDate { get; set; }

    public DateTime? NextContactDate { get; set; }

    // Computed by CompanyService against the caller's local date; not a column.
    public NextContactDateStatus NextContactDateStatus { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
}
