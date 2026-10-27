using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class WaterSupplierLetterSettingsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }

    public BackflowExpiringType? ExpiringNotice1 { get; set; }
    public BackflowExpiringType? ExpiringNotice2 { get; set; }
    public BackflowExpiredType? ExpiredNotice1 { get; set; }
    public BackflowExpiredType? ExpiredNotice2 { get; set; }
    public BackflowNonCompliantType? BackflowNonCompliant1 { get; set; }
    public BackflowNonCompliantType? BackflowNonCompliant2 { get; set; }
    public bool? BackflowNonCompliantPrint1 { get; set; }
    public bool? BackflowNonCompliantPrint2 { get; set; }
    public bool? UnknownAssembliesNotice { get; set; }

    public CsiImpendingType? ImpendingNotice1 { get; set; }
    public CsiImpendingType? ImpendingNotice2 { get; set; }
    public CsiPastDueType? PastDueNotice1 { get; set; }
    public CsiPastDueType? PastDueNotice2 { get; set; }
    public CsiNonCompliantType? CsiNonCompliant1 { get; set; }
    public CsiNonCompliantType? CsiNonCompliant2 { get; set; }
    public bool? CsiNonCompliantPrint1 { get; set; }
    public bool? CsiNonCompliantPrint2 { get; set; }
}
