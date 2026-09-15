using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Backflow;

// One row per water supplier per month, capturing that month's backflow compliance counts as of the
// 1st. The natural composite key (WaterSupplierId, ReportDate) enforces a single snapshot per month, so
// no surrogate Id is needed. WaterSupplierId is inherited from TenantModel<WaterSupplier>.
[Table("BackflowComplianceSnapshots")]
public class BackflowComplianceSnapshot : TenantModel<WaterSupplier>
{
    // Always the first day of the month the snapshot represents (mirrors V1's snapshot-on-day-1).
    [AppPrimaryKey(false)]
    [Column(TypeName = "date")]
    public DateTime ReportDate { get; set; }

    public int Total { get; set; }

    public int Compliant { get; set; }
}
