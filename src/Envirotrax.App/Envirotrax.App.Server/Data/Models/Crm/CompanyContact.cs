using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Crm;

/// <summary>
/// Another person to contact at a CRM company (V1 Vepo Manager's Additional Contacts,
/// ContactManagerAdditionalContacts). V1 deletes these rows and records only when they were created, so they have
/// creation and update audit but no soft delete: a deleted contact is removed.
/// </summary>
[Table("CompanyContacts", Schema = "crm")]
public class CompanyContact : TenantModel<WaterSupplier>, ICreateAuditableModel<AppUser>, IUpdateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    // The contact belongs to its company's water supplier, which is part of this key.
    public int CompanyId { get; set; }
    public Company? Company { get; set; }

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

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? CellNumber { get; set; }

    [MaxLength(100)]
    public string? EmailAddress { get; set; }

    // Audit
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? UpdatedById { get; set; }
    public AppUser? UpdatedBy { get; set; }
    public DateTime? UpdatedTime { get; set; }
}
