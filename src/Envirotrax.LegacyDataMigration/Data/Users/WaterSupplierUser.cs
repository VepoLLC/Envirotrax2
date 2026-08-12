using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Envirotrax.LegacyDataMigration.Data;

namespace Envirotrax.LegacyDataMigration.Data.Users;

public class WaterSupplierUser : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AspNetUserBase? User { get; set; }

    [Required]
    [StringLength(100)]
    public string ContactName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string EmailAddress { get; set; } = null!;

    [StringLength(25)]
    public string? CellNumber { get; set; }

    // Points back to Vepo.dbo.WaterSupplierUserAccounts.ID - the source of truth for this user's
    // legacy data (including the permission columns used by the one-time role migration).
    public int? LegacyRecordId { get; set; }
}
