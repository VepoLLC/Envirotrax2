namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

public class WaterSupplierLetterSettingsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }

    public int? ExpiringNotice1 { get; set; }
    public int? ExpiringNotice2 { get; set; }
    public int? ExpiredNotice1 { get; set; }
    public int? ExpiredNotice2 { get; set; }
    public int? BackflowNonCompliant1 { get; set; }
    public int? BackflowNonCompliant2 { get; set; }
    public bool? BackflowNonCompliantPrint1 { get; set; }
    public bool? BackflowNonCompliantPrint2 { get; set; }
    public bool? UnknownAssembliesNotice { get; set; }

    public int? ImpendingNotice1 { get; set; }
    public int? ImpendingNotice2 { get; set; }
    public int? PastDueNotice1 { get; set; }
    public int? PastDueNotice2 { get; set; }
    public int? CsiNonCompliant1 { get; set; }
    public int? CsiNonCompliant2 { get; set; }
    public bool? CsiNonCompliantPrint1 { get; set; }
    public bool? CsiNonCompliantPrint2 { get; set; }
}
