using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Crm;

/// <summary>
/// A company tracked in the CRM (V1 Vepo Manager's Contact Manager, ContactManagerContacts). Vepo's customers
/// are water suppliers and private companies, so a V1 "contact" is a company. The records the Envirotrax.Admin
/// users add belong to the Vepo LLC water supplier, and a water supplier could keep its own later.
/// </summary>
[Table("Companies", Schema = "crm")]
public class Company : TenantModel<WaterSupplier>, IAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public ContactType ContactType { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(100)]
    public string? Position { get; set; }

    [MaxLength(100)]
    public string? ContactName { get; set; }

    [MaxLength(100)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    public int? StateId { get; set; }
    public State? State { get; set; }

    [MaxLength(50)]
    public string? ZipCode { get; set; }

    [MaxLength(100)]
    public string? EmailAddress { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? CellNumber { get; set; }

    public bool HasBackflowTesting { get; set; }
    public DateTime? BackflowTestingStartDate { get; set; }

    public bool HasCsiInspection { get; set; }
    public DateTime? CsiInspectionStartDate { get; set; }

    public bool HasFogInspection { get; set; }
    public DateTime? FogInspectionStartDate { get; set; }

    public bool HasFogTransportation { get; set; }
    public DateTime? FogTransportationStartDate { get; set; }

    public int? LastContactedById { get; set; }
    public AppUser? LastContactedBy { get; set; }

    public DateTime? LastContactDate { get; set; }

    public DateTime? NextContactDate { get; set; }

    // Audit
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? UpdatedById { get; set; }
    public AppUser? UpdatedBy { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}
