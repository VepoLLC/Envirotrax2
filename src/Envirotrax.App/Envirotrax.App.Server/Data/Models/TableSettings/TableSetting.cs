using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.TableSettings;

/// <summary>
/// A water supplier user's saved settings for one table, such as a backflow test search template.
/// </summary>
/// <remarks>
/// Scoped by water supplier on purpose. Professionals will get their own ProfessionalTableSettings table.
/// </remarks>
[Table("TableSettings")]
[AppIndex(nameof(UserId), nameof(TableType))]
public class TableSetting : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public TableSettingType TableType { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public int UserId { get; set; }
    public WaterSupplierUser? User { get; set; }

    // JSON written by the client: { "query": <the table's query>, ...extra parameters of the table }
    public string? Filters { get; set; }
}
