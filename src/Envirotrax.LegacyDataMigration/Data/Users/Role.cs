
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Envirotrax.LegacyDataMigration.Data;

namespace Envirotrax.LegacyDataMigration.Data.Users;

public class Role : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
}